using Shared.DataTransferObjects.Employee;

namespace Shared.DataTransferObjects.Company;

public record CompanyForCreationDto(string Name, string Address, string Country,
    IEnumerable<EmployeeForCreationDto> Employees);
