using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BGU.Api.Filters;

public class ResponseStatusCodeFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value != null)
        {
            var valueType = objectResult.Value.GetType();
            var statusCodeProperty = valueType.GetProperty("StatusCode");
            
            if (statusCodeProperty != null)
            {
                var statusCode = statusCodeProperty.GetValue(objectResult.Value);
                if (statusCode is int code)
                {
                    objectResult.StatusCode = code;
                }
            }
        }
    }
}