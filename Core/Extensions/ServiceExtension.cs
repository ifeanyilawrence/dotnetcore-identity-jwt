using Core.Account.Implementation;
using Core.Account.Interfaces;
using Data.Repositories.Implementation;
using Data.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection InjectDependencies(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped<IAccountLogic, AccountLogic>();

            return services;
        }
    }
}
