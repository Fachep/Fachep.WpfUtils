using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
public class WithViewModelAttribute(Type viewModelType) : Attribute
{
    public Type ViewModelType => viewModelType;
    public ServiceLifetime LifeTime { get; set; } = ServiceLifetime.Singleton;
    public bool IsDefault { get; set; } = false;
}