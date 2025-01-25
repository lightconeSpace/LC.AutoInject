using System.Reflection;
using System.Runtime.Loader;
using AutoInject.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq.Extensions;

namespace AutoInject.Extensions;

public static class ServiceCollectionExtension
{
     public static void AddAutoInjectComponents(this IServiceCollection services)
    {
        LoadAllAssemblies();
        AppDomain.CurrentDomain.GetAssemblies().ForEach(item => item.GetTypes()
            .Where(t => t.IsClass && Attribute.IsDefined(t, typeof(AutoInjectAttribute))).ToList().ForEach(
                implementationType =>
                {
                    var interfaceTypes = implementationType.GetInterfaces();
                    var diAttribute = implementationType.GetCustomAttribute<AutoInjectAttribute>();

                    services.AddBean(implementationType, implementationType, diAttribute!.Lifetime);

                    foreach (var interfaceType in interfaceTypes)
                    {
                        services.AddBean(interfaceType, implementationType, diAttribute!.Lifetime);
                    }
                }));
    }

    private static void AddBean(this IServiceCollection services, Type serviceType, Type implementationType,
        ServiceLifetime lifetime)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton(serviceType, implementationType);

                return;
            case ServiceLifetime.Scoped:
                services.AddScoped(serviceType, implementationType);

                return;
            case ServiceLifetime.Transient:
                services.AddTransient(serviceType, implementationType);

                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private static void LoadAllAssemblies()
    {
        string path = AppContext.BaseDirectory;

        foreach (string dll in Directory.GetFiles(path, "*.dll"))
        {
            try
            {
                if (!AssemblyLoadContext.Default.Assemblies.Any(a => a.GetName().Name == Path.GetFileNameWithoutExtension(dll)))
                {
                    Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" {dll}: {ex.Message}");
            }
        }
    }
}