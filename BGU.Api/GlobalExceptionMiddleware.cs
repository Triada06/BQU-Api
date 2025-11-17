namespace BGU.Api;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var response = new
            {
                StatusCode = 500,
                Message = "Internal server error",
                Success = false
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}