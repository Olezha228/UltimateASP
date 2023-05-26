using Microsoft.AspNetCore.Http;
using Shared.RequestFeatures;

namespace Entities.LinkModels;

public record LinkParameters(EmployeeRequestParameters EmployeeParameters,
    HttpContext Context);
