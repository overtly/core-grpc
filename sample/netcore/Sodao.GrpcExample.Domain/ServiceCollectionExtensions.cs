using Microsoft.Extensions.DependencyInjection;
using Sodao.GrpcExample.Domain.Contracts;
using Sodao.GrpcExample.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sodao.GrpcExample.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDomainDI(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
        }
    }
}
