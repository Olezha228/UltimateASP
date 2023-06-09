﻿using System.Text.Json;
using CompanyEmployees.Presentation.ActionFilters;
using Entities.LinkModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Manager;
using Shared.DataTransferObjects.Employee;
using Shared.RequestFeatures;
// ReSharper disable RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId:guid}/employees")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IServiceManager _service;

    public EmployeesController(IServiceManager service) => _service = service;

    [HttpGet]
    [HttpHead]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
        [FromQuery] EmployeeRequestParameters employeeParameters)
    {
        var linkParams = new LinkParameters(employeeParameters, HttpContext);

        var (linkResponse, metaData) = await _service.EmployeeService.GetEmployeesAsync(companyId,
            linkParams, trackChanges: false);

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(metaData));

        return linkResponse.HasLinks
            ? Ok(linkResponse.LinkedEntities)
            : Ok(linkResponse.ShapedEntities);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
    {
        var employee = await _service.EmployeeService.GetEmployeeAsync(companyId, id, trackChanges: false);

        return Ok(employee);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeForCreation)
    {
        var employee =
            await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employeeForCreation, trackChanges: false);

        if (employee is null)
        {
            return BadRequest("EmployeeDto object is null");
        }

        // ReSharper disable once RedundantAnonymousTypePropertyName
        return CreatedAtRoute(nameof(GetEmployeeForCompany), new { companyId = companyId, id = employee.Id }, employee);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
    {
        await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges: false);

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] EmployeeForUpdateDto employee)
    {
        await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee, companyTrackChanges: false, employeeTrackChanges: true);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    [ServiceFilter(typeof(JsonPatchValidationFilter))]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDocument)
    {
        var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id,
            companyTrackChanges: false,
            employeeTrackChanges: true);

        patchDocument.ApplyTo(result.employeeToPatch);

        TryValidateModel(result.employeeToPatch);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity);

        return NoContent();
    }
}