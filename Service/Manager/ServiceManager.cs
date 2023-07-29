using AutoMapper;
using Contracts.Contracts;
using Contracts.Manager;
using Entities.ConfigurationModels;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Service.Contracts.Manager;
using Service.Contracts.ServiceInterfaces;

namespace Service.Manager;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<ICompanyService> _companyService;
    private readonly Lazy<IEmployeeService> _employeeService;
    private readonly Lazy<IAuthenticationService> _authenticationService;

    public ServiceManager(IRepositoryManager repositoryManager, 
        ILoggerManager logger,
        IMapper mapper, IEmployeeLinks employeeLinks, 
        ServiceHelper serviceHelper,
        UserManager<User> userManager,
        IOptionsMonitor<JwtConfiguration> configuration)
    {
        _companyService = new Lazy<ICompanyService>(() =>
            new CompanyService(repositoryManager, logger, mapper, serviceHelper));

        _employeeService = new Lazy<IEmployeeService>(() =>
            new EmployeeService(repositoryManager, logger, mapper, employeeLinks, serviceHelper));

        _authenticationService = new Lazy<IAuthenticationService>(() =>
            new AuthenticationService(logger, mapper, userManager, configuration));
    }

    public ICompanyService CompanyService => _companyService.Value;

    public IEmployeeService EmployeeService => _employeeService.Value;

    public IAuthenticationService AuthenticationService =>
        _authenticationService.Value;

}