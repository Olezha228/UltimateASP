using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Employee;

namespace Service.Contracts.ServiceInterfaces;

public interface IEmployeeService
{
    IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges);

    EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges);

    EmployeeDto CreateEmployeeForCompany(Guid companyId, 
        EmployeeForCreationDto employeeForCreation, bool trackChanges);

    void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges);

    void UpdateEmployeeForCompany(Guid companyId, Guid id,
        EmployeeForUpdateDto? employeeForUpdate,
        bool companyTrackChanges, bool employeeTrackChanges);
}