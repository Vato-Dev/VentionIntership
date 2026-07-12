using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middlewares
{
    public class NotFoundExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler // it's more likely part of globalexception overkill to make it separate handler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not NotFoundException)
                return false;
            
      
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Type = exception.GetType().Name, 
                    Title = $"The entity  could not be found",
                    Detail = exception.Message
                }
            });
        }
    }
}
