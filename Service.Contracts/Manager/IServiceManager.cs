using Service.Contracts.Services;

namespace Service.Contracts.Manager;

public interface IServiceManager
{
    ICompanyService CompanyService { get; }

    IEmployeeService EmployeeService { get; }
}