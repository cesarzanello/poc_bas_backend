using Microsoft.Extensions.DependencyInjection;
using Application.Commond.Interface;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using System;

namespace Infrastructure.Boopstrap
{
    public static class ScopedServicesRegister
    {
        public static IServiceCollection AddBackendScopedServices(this IServiceCollection services)
        {

            var projectNames = DependencyContext.Default
                .CompileLibraries
                .Where(lib => lib.Type.Equals("project", StringComparison.OrdinalIgnoreCase))
                .Select(lib => lib.Name)
                .ToList();

            var scopedServices = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(x => projectNames.Any(proj => string.Equals(x.GetName().Name, proj,StringComparison.OrdinalIgnoreCase)))
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IScopedService).IsAssignableFrom(x))
            .ToList();

            var scopedServicesImplementations = scopedServices.Where(x => x.IsClass && !x.IsAbstract);
            var scopedServicesInterfaces = scopedServices.Where(x => x.IsInterface);

            foreach (var scopedServiceImplementation in scopedServicesImplementations)
            {
                var serviceInterface = scopedServicesInterfaces.FirstOrDefault(x => x.Name == $"I{scopedServiceImplementation.Name}");

                if (serviceInterface == null)
                {
                    continue;
                }

                services.AddScoped(serviceInterface, scopedServiceImplementation);

            }
            return services;
        }


        public static string ObtenerNombreSolucion()
        {
            var entry = Assembly.GetEntryAssembly();
            return entry?
              .GetCustomAttributes<AssemblyMetadataAttribute>()
              .FirstOrDefault(a => a.Key == "ZarcomTemplate")
              ?.Value;
        }
    }
}
