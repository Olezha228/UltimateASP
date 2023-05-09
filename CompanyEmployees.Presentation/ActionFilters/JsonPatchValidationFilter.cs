using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

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