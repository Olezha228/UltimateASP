﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts.ServiceInterfaces;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Token;

namespace Service;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly IOptionsMonitor<JwtConfiguration> _configuration;
    private readonly JwtConfiguration _jwtConfiguration;

    private User? _user;

    internal AuthenticationService(ILoggerManager logger, IMapper mapper,
        UserManager<User> userManager,
        IOptionsMonitor<JwtConfiguration> configuration)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
        _jwtConfiguration = _configuration.CurrentValue;
    }

    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var user = _mapper.Map<User>(userForRegistration);

        var result = await _userManager.CreateAsync(user,
            userForRegistration.Password!);

        //If you want, before calling AddToRoleAsync or AddToRolesAsync, you
        //can check if roles exist in the database. But for that, you have to inject
        //RoleManager < TRole > and use the RoleExistsAsync method.

        if (result.Succeeded && userForRegistration.Roles is not null)
        {
            await _userManager.AddToRolesAsync(user, userForRegistration.Roles);
        }

        return result;
    }

    public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
    {
        _user = await _userManager.FindByNameAsync(userForAuth.UserName!);

        var isResultValid = IsPasswordMatches(userForAuth);

        if (!isResultValid.Result)
        {
            _logger.LogWarn(
                $"{nameof(ValidateUser)}:" +
                "Authentication failed. Wrong user name or password.");
        }

        return isResultValid.Result;
    }

    private async Task<bool> IsPasswordMatches(UserForAuthenticationDto userForAuth)
    {
        return (_user != null) && await _userManager.CheckPasswordAsync(_user, userForAuth.Password!);
    }

    public async Task<TokenDto> CreateToken(bool isUpdateTokenExpiryTime)
    {
        if (_user == null || _user.UserName == null)
        {
            throw new Exception("Invalid user!");
        }

        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        var refreshToken = GenerateRefreshToken();

        _user.RefreshToken = refreshToken;

        if (isUpdateTokenExpiryTime)
        {
            _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        }

        await _userManager.UpdateAsync(_user);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, refreshToken);
    }

    private static SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")!);

        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, _user!.UserName!)
        };

        var roles = await _userManager.GetRolesAsync(_user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials,
        IEnumerable<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken
        (
            issuer: _jwtConfiguration.ValidIssuer,
            audience: _jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
            signingCredentials: signingCredentials
        );

        return tokenOptions;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")!)),

            ValidateLifetime = true,
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !IsHmacSha256Algorithm(jwtSecurityToken))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    private static bool IsHmacSha256Algorithm(JwtSecurityToken jwtSecurityToken)
    {
        return jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase);
    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

        if (principal.Identity?.Name is null)
        {
            throw new RefreshTokenBadRequest();
        }

        var user = await _userManager.FindByNameAsync(principal.Identity.Name);

        if (!IsUserExist(user) || !AreRefreshTokensEqual(tokenDto, user!) ||
            IsRefreshTokenExpired(user!))
        {
            throw new RefreshTokenBadRequest();
        }

        _user = user;

        return await CreateToken(isUpdateTokenExpiryTime: false);
    }

    private static bool IsRefreshTokenExpired(User user)
    {
        return user.RefreshTokenExpiryTime <= DateTime.Now;
    }

    private static bool AreRefreshTokensEqual(TokenDto tokenDto, User user)
    {
        return user.RefreshToken == tokenDto.RefreshToken;
    }

    private static bool IsUserExist(User? user)
    {
        return user != null;
    }
}
