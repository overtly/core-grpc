using Microsoft.Extensions.DependencyInjection;
using Sodao.GrpcExample.Application.Constracts;
using Sodao.GrpcExample.Application.Services;
using Sodao.GrpcExample.Domain;

namespace Sodao.GrpcExample.Application
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
