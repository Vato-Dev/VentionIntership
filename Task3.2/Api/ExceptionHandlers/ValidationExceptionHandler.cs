using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.Middlewares
{
    public sealed class ValidationExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            if (exception is not ValidationException validationException)
                return false;

            var statusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.StatusCode = statusCode;

            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var problemDetails = new HttpValidationProblemDetails(errors)
            {
                Status = statusCode,
                Type = exception.GetType().Name,
                Title = "One or more validation errors occurred.",
                Detail = "Please refer to the errors property for additional details.",
                Instance = httpContext.Request.Path
            };

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails
            });
        }
    }
}
