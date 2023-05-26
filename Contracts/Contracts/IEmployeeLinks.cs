using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.LinkModels;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects.Employee;

namespace Contracts.Contracts;

public interface IEmployeeLinks
{
    LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto,
        string? fields, Guid companyId, HttpContext httpContext);
}

