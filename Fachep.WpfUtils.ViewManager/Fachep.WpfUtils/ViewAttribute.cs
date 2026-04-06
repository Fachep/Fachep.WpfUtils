using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ViewAttribute(params string[] names) : Attribute
{
    public string[] Names => names;
    public ServiceLifetime LifeTime { get; set; } = ServiceLifetime.Singleton;
    public Type? ViewType { get; set; }
}