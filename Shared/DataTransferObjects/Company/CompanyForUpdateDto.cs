using Shared.DataTransferObjects.Employee;

namespace Shared.DataTransferObjects.Company;

public record CompanyForUpdateDto(string Name, string Address, string Country,
    IEnumerable<EmployeeForCreationDto> Employees);