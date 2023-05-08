using AutoMapper;
using Contracts.Manager;
using LoggerService;
using Service.Contracts.Manager;
using Service.Contracts.ServiceInterfaces;

namespace Service.Manager;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<ICompanyService> _companyService;
    private readonly Lazy<IEmployeeService> _employeeService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper, ServiceHelper serviceHelper)
    {
        _companyService = new Lazy<ICompanyService>(() => 
            new CompanyService(repositoryManager, logger, mapper, serviceHelper));

        _employeeService = new Lazy<IEmployeeService>(() =>
            new EmployeeService(repositoryManager, logger, mapper, serviceHelper));
    }

    public ICompanyService CompanyService => _companyService.Value;

    public IEmployeeService EmployeeService => _employeeService.Value;
}