using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts.Contracts;

public interface IEmployeeRepository
{
    Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,
        EmployeeRequestParameters employeeParameters, bool trackChanges);

    Task<Employee?> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);

    void CreateEmployeeForCompany(Guid companyId, Employee employee);

    void DeleteEmployee(Employee employee);
}