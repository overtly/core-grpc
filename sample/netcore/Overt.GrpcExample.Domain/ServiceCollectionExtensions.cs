using Microsoft.Extensions.DependencyInjection;
using Overt.GrpcExample.Domain.Contracts;
using Overt.GrpcExample.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overt.GrpcExample.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDomainDI(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
        }
    }
}
