using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.ExceptionHandlers
{
    public  sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) :IExceptionHandler
    { 
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
         
            httpContext.Response.Clear();

            httpContext.Response.StatusCode = exception switch
            {
                _ => StatusCodes.Status500InternalServerError
            };

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails()
                {
                    Type = exception.GetType().Name, 
                    Title = "Some Error occured contact support or just wait , i dunno what u can else do :)",
                    Detail = exception.Message
                }
            });
        }
    }
}
