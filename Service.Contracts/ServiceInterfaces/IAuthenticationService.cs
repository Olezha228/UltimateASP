using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts.ServiceInterfaces;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);
    
    Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);

    Task<TokenDto> CreateToken(bool isUpdateTokenExpiryTime);

    Task<TokenDto> RefreshToken(TokenDto tokenDto);
}
