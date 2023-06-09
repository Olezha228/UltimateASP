﻿using Contracts.Contracts;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects.Employee;
#pragma warning disable CS8600

namespace UltimateASP.Utility;

public class EmployeeLinks : IEmployeeLinks
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IDataShaper<EmployeeDto> _dataShaper;

    public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDto>
        dataShaper)
    {
        _linkGenerator = linkGenerator;
        _dataShaper = dataShaper;
    }

    public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto,
        string? fields, Guid companyId, HttpContext httpContext)
    {
        var employeeDtos = employeesDto.ToList();
        var shapedEmployees = ShapeData(employeeDtos, fields);

        if (ShouldGenerateLinks(httpContext))
        {
            return ReturnLinkedEmployees(employeeDtos,
                fields, companyId, httpContext, shapedEmployees);
        }

        return ReturnShapedEmployees(shapedEmployees);
    }

    private List<Entity> ShapeData(IEnumerable<EmployeeDto> employeesDto, string? fields)
        => _dataShaper.ShapeData(employeesDto, fields)
                .Select(e => e.Entity)
                .ToList();

    private static bool ShouldGenerateLinks(HttpContext httpContext)
    {
        var mediaType = (MediaTypeHeaderValue) httpContext.Items["AcceptHeaderMediaType"];

        if (mediaType == null)
        {
            return false;
        }

        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas",
            StringComparison.InvariantCultureIgnoreCase);
    }

    private static LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees)
        => new() { ShapedEntities = shapedEmployees };

    private LinkResponse ReturnLinkedEmployees(IEnumerable<EmployeeDto> employeesDto,
        string? fields, Guid companyId, HttpContext httpContext, List<Entity> shapedEmployees)
    {
        var employeeDtoList = employeesDto.ToList();

        for (var index = 0; index < employeeDtoList.Count; index++)
        {
            var employeeLinks = CreateLinksForEmployee(httpContext, companyId,
                employeeDtoList[index].Id, fields);

            shapedEmployees[index].Add("Links", employeeLinks);
        }

        var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);
        var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);

        return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
    }

    private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId,
        Guid id, string? fields = "")
    {
        fields ??= string.Empty;

        var links = new List<Link>
        {
            new(_linkGenerator.GetUriByAction(httpContext, "GetEmployeeForCompany",
                    values: new { companyId, id, fields }),
                "self",
                "GET"),

            new(_linkGenerator.GetUriByAction(httpContext,
                    "DeleteEmployeeForCompany", values: new { companyId, id }),
                "delete_employee",
                "DELETE"),

            new(_linkGenerator.GetUriByAction(httpContext, "UpdateEmployeeForCompany",
                values : new { companyId, id }),
                "update_employee", 
                "PUT"),

            new(_linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateEmployeeForCompany",
                values : new { companyId, id }),
                "partially_update_employee",
                "PATCH")
        };

        return links;
    }

    private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext,
        LinkCollectionWrapper<Entity> employeesWrapper)
    {
        employeesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext,
                "GetEmployeesForCompany", values: new { }),
            "self",
            "GET"));
        return employeesWrapper;
    }
}