using Application.Abstractions;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.SericeCollectionExtension
{
    public static class ServiceCollectionExtension
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>()
                .AddScoped<IOrganizationService, OrganizationService>()
                .AddScoped<ISessionService, SessionService>()
                .AddScoped<IMembershipService, MembershipService>();
        }
    }
}
