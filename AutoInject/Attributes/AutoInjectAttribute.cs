using Microsoft.Extensions.DependencyInjection;

namespace AutoInject.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AutoInjectAttribute : System.Attribute
{
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;
    
}