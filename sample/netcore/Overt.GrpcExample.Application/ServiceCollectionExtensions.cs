using Microsoft.Extensions.DependencyInjection;
using Overt.GrpcExample.Application.Constracts;
using Overt.GrpcExample.Application.Services;
using Overt.GrpcExample.Domain;

namespace Overt.GrpcExample.Application
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationDI(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();

            services.AddDomainDI();
        }
    }
}
