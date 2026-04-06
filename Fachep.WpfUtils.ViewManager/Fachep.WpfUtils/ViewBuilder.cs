using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils;

public class ViewBuilder
{
    private readonly IServiceCollection _services;
    private readonly Type _viewType;

    private ViewBuilder(Type viewType, IServiceCollection services)
    {
        _viewType = viewType;
        _services = services;
    }

    private static ViewBuilder Create(Type viewType, IServiceCollection services, ServiceLifetime viewLifetime,
        IViewWrapper viewWrapper)
    {
        var wrapperType = typeof(IViewWrapper<>).MakeGenericType(viewType);
        services
            .AddTransient(viewType, sp => ((IViewWrapper)sp.GetRequiredService(wrapperType)).GetInstance(sp))
            .Add(new ServiceDescriptor(wrapperType, _ => viewWrapper.Clone(), viewLifetime));
        return Create(viewType, services);
    }

    internal static ViewBuilder Create(Type viewType, IServiceCollection services)
    {
        services.AddSingleton<ViewManager>();
        return new ViewBuilder(viewType, services);
    }

    internal static ViewBuilder Create(Type viewType, IServiceCollection services, object viewInstance)
    {
        var wrapperType = typeof(InstanceViewWrapper<>).MakeGenericType(viewType);
        var wrapper = (IViewWrapper)Activator.CreateInstance(wrapperType, viewInstance)!;
        return Create(viewType, services, ServiceLifetime.Singleton, wrapper);
    }

    internal static ViewBuilder Create(Type viewType, IServiceCollection services,
        ServiceLifetime viewLifetime, Func<IServiceProvider, FrameworkElement> factory)
    {
        var wrapperType = typeof(FactoryViewWrapper<>).MakeGenericType(viewType);
        var wrapper = (IViewWrapper)Activator.CreateInstance(wrapperType, factory)!;
        return Create(viewType, services, viewLifetime, wrapper);
    }

    internal static ViewBuilder Create(Type viewType, IServiceCollection services, ServiceLifetime viewLifetime,
        Type implType)
    {
        var wrapperType = typeof(TypeViewWrapper<>).MakeGenericType(viewType);
        var wrapper = (IViewWrapper)Activator.CreateInstance(wrapperType, implType)!;
        return Create(viewType, services, viewLifetime, wrapper);
    }

    public ViewBuilder WithViewModel(Type viewModelType)
    {
        _services.AddSingleton(typeof(IViewModelConfiguration<>).MakeGenericType(viewModelType),
            new ViewModelConfiguration(_viewType));
        return this;
    }

    public ViewBuilder WithName(string name)
    {
        _services.AddKeyedSingleton<IViewNameConfiguration>(name, new ViewNameConfiguration(_viewType));
        return this;
    }

    private interface IViewWrapper
    {
        FrameworkElement GetInstance(IServiceProvider serviceProvider);
        IViewWrapper Clone();
    }

    private interface IViewWrapper<out TView> : IViewWrapper
        where TView : FrameworkElement
    {
    }

    private abstract class ViewWrapper<TView> : IViewWrapper<TView>
        where TView : FrameworkElement
    {
#if NET9_0_OR_GREATER
        private readonly Lock _lock = new();
#else
        private readonly object _lock = new();
#endif
        private TView? _instance;

        FrameworkElement IViewWrapper.GetInstance(IServiceProvider serviceProvider)
        {
            if (_instance == null)
                lock (_lock)
                {
                    _instance ??= CreateInstance(serviceProvider);
                }

            return _instance;
        }

        IViewWrapper IViewWrapper.Clone()
        {
            return Clone();
        }

        protected abstract TView CreateInstance(IServiceProvider serviceProvider);

        protected abstract ViewWrapper<TView> Clone();
    }

    private class TypeViewWrapper<TView> : ViewWrapper<TView>
        where TView : FrameworkElement
    {
        private readonly ObjectFactory _factory;
        private readonly Type _implType;

        private TypeViewWrapper(Type implType, ObjectFactory factory)
        {
            _implType = implType;
            _factory = factory;
        }

        public TypeViewWrapper(Type implType) : this(implType, ActivatorUtilities.CreateFactory(implType, []))
        {
        }

        protected override TView CreateInstance(IServiceProvider serviceProvider)
        {
            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher.CheckAccess()) return (TView)_factory(serviceProvider, []);

            return (TView)dispatcher.Invoke(() => _factory(serviceProvider, []));
        }

        protected override ViewWrapper<TView> Clone()
        {
            return new TypeViewWrapper<TView>(_implType, _factory);
        }
    }

    private class FactoryViewWrapper<TView>(Func<IServiceProvider, TView> factory) : ViewWrapper<TView>
        where TView : FrameworkElement
    {
        protected override TView CreateInstance(IServiceProvider serviceProvider)
        {
            return factory(serviceProvider);
        }

        protected override ViewWrapper<TView> Clone()
        {
            return new FactoryViewWrapper<TView>(factory);
        }
    }

    private class InstanceViewWrapper<TView>(TView instance) : IViewWrapper<TView>
        where TView : FrameworkElement
    {
        FrameworkElement IViewWrapper.GetInstance(IServiceProvider serviceProvider)
        {
            return instance;
        }

        IViewWrapper IViewWrapper.Clone()
        {
            return new InstanceViewWrapper<TView>(instance);
        }
    }

    internal interface IViewModelConfiguration<in TViewModel>
    {
        Type ViewType { get; }
    }

    internal interface IViewNameConfiguration
    {
        Type ViewType { get; }
    }

    private class ViewModelConfiguration(Type viewType) : IViewModelConfiguration<object>
    {
        public Type ViewType => viewType;
    }

    private class ViewNameConfiguration(Type viewType) : IViewNameConfiguration
    {
        public Type ViewType => viewType;
    }
}