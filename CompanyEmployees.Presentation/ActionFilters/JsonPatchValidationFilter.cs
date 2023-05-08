using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.ActionFilters;

public class JsonPatchValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue("patchDocument", out var patchDocument) || patchDocument == null)
        {
            context.ModelState.AddModelError("patchDocument", "The patchDocument object sent from the client is null.");
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}