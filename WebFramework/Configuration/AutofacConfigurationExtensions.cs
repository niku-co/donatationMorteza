﻿using System.Reflection;
using Autofac;
using Common;
using Data;
using Data.Contracts;
using Data.Repositories;
using Entities.Common;
using MediatR.Extensions.Autofac.DependencyInjection;
using Services.Services;

namespace WebFramework.Configuration
{
    public static class AutofacConfigurationExtensions
    {
        public static void AddServices(this ContainerBuilder containerBuilder)
        {
            //RegisterType > As > Liftetime
            containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            var commonAssembly = typeof(SiteSettings).Assembly;
            var entitiesAssembly = typeof(IEntity).Assembly;
            var dataAssembly = typeof(ApplicationDbContext).Assembly;
            var servicesAssembly = typeof(JwtService).Assembly;
            
            //containerBuilder.RegisterMediatR(Assembly.GetEntryAssembly());

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<IScopedDependency>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<ITransientDependency>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<ISingletonDependency>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }

        //We don't need this since Autofac updates for ASP.NET Core 3.0+ Generic Hosting
        //public static IServiceProvider BuildAutofacServiceProvider(this IServiceCollection services)
        //{
        //    var containerBuilder = new ContainerBuilder();
        //    containerBuilder.Populate(services);
        //
        //    //Register Services to Autofac ContainerBuilder
        //    containerBuilder.AddServices();
        //
        //    var container = containerBuilder.Build();
        //    return new AutofacServiceProvider(container);
        //}
    }
}
