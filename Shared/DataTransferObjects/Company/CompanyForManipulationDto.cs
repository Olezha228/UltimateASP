using Shared.DataTransferObjects.Employee;
using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedMember.Global

namespace Shared.DataTransferObjects.Company;

public record CompanyForManipulationDto
{
    [Required(ErrorMessage = "Company name is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
    public string? Name { get; init; }

    [Required(ErrorMessage = "Company address is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Address is 60 characters")]
    public string? Address { get; init; }

    [Required(ErrorMessage = "Company address is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Address is 60 characters")]
    public string? Country { get; init; }

    public ICollection<EmployeeForCreationDto>? Employees { get; init; }
}
