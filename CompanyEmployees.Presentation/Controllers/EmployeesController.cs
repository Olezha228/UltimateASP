using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Manager;
using Shared.DataTransferObjects.Employee;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IServiceManager _service;

    public EmployeesController(IServiceManager service) => _service = service;

    [HttpGet]
    public IActionResult GetEmployeesForCompany(Guid companyId)
    {
        var employees = _service.EmployeeService.GetEmployees(companyId, trackChanges: false);

        return Ok(employees);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
    public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
    {
        var employee = _service.EmployeeService.GetEmployee(companyId, id, trackChanges: false);

        return Ok(employee);
    }

    [HttpPost]
    public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto? employeeForCreation)
    {
        if (employeeForCreation is null)
        {
            return BadRequest("EmployeeForCreationDto object is null");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var employee =
            _service.EmployeeService.CreateEmployeeForCompany(companyId, employeeForCreation, trackChanges: false);

        if (employee is null)
        {
            return BadRequest("EmployeeDto object is null");
        }

        // ReSharper disable once RedundantAnonymousTypePropertyName
        return CreatedAtRoute(nameof(GetEmployeeForCompany), new { companyId = companyId, id = employee.Id }, employee);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)
    {
        _service.EmployeeService.DeleteEmployeeForCompany(companyId, id, trackChanges: false);

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] EmployeeForUpdateDto? employee)
    {
        if (employee is null)
        {
            return BadRequest("EmployeeForUpdateDto object is null");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        _service.EmployeeService.UpdateEmployeeForCompany(companyId, id, employee,
            companyTrackChanges: false, employeeTrackChanges: true);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto>? patchDocument)
    {
        if (patchDocument is null)
        {
            return BadRequest("patchDocument object sent from client is null.");
        }

        var result = _service.EmployeeService.GetEmployeeForPatch(companyId, id,
            companyTrackChanges: false,
            employeeTrackChanges: true);

        patchDocument.ApplyTo(result.employeeToPatch, ModelState);

        TryValidateModel(result.employeeToPatch);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        _service.EmployeeService.SaveChangesForPatch(result.employeeToPatch, result.employeeEntity);

        return NoContent();
    }
}