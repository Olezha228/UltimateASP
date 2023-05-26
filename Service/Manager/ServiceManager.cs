using AutoMapper;
using Contracts.Contracts;
using Contracts.Manager;
using LoggerService;
using Service.Contracts.Manager;
using Service.Contracts.ServiceInterfaces;
using Shared.DataTransferObjects.Employee;

namespace Service.Manager;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<ICompanyService> _companyService;
    private readonly Lazy<IEmployeeService> _employeeService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger,
        IMapper mapper, IEmployeeLinks employeeLinks, ServiceHelper serviceHelper)
    {
        _companyService = new Lazy<ICompanyService>(() =>
            new CompanyService(repositoryManager, logger, mapper, serviceHelper));

        _employeeService = new Lazy<IEmployeeService>(() =>
            new EmployeeService(repositoryManager, logger, mapper, employeeLinks, serviceHelper));
    }


    public ICompanyService CompanyService => _companyService.Value;

    public IEmployeeService EmployeeService => _employeeService.Value;
}