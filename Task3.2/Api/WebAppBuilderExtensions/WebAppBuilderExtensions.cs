namespace Api.WebAppBuilderExtensions
{
    public static class WebAppBuilderExtensions
    {
        public static void ConfigureProblemDetails(this WebApplicationBuilder builder)
        {
            builder.Services.AddProblemDetails(op => {
                op.CustomizeProblemDetails = context => {
                    context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                };
            });
        }
    }
}
