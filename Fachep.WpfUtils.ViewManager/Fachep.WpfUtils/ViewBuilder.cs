using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using Expression = System.Linq.Expressions.Expression;

namespace Fachep.WpfUtils;

public class ViewBuilder
{
    private readonly IServiceCollection _services;
    private readonly Type _viewType;
    private IViewWrapper? _viewWrapper;

    private ViewBuilder(Type viewType, IServiceCollection services)
    {
        _viewType = viewType;
        _services = services;
    }

    private static ViewBuilder Create(
        Type viewType,
        IServiceCollection services,
        ServiceLifetime viewLifetime,
        IViewWrapper viewWrapper
    )
    {
        var wrapperType = typeof(IViewWrapper<>).MakeGenericType(viewType);
        services
            .AddTransient(viewType, sp => ((IViewWrapper)sp.GetRequiredService(wrapperType)).GetInstance(sp))
            .Add(new ServiceDescriptor(wrapperType, _ => viewWrapper.Clone(), viewLifetime));
        var builder = Create(viewType, services);
        builder._viewWrapper = viewWrapper;
        return builder;
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

    internal static ViewBuilder Create(
        Type viewType,
        IServiceCollection services,
        ServiceLifetime viewLifetime,
        Func<IServiceProvider, FrameworkElement> factory
    )
    {
        var wrapperType = typeof(FactoryViewWrapper<>).MakeGenericType(viewType);
        var adapterType = typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), viewType);
        Delegate adapter;
        try
        {
            adapter = Delegate.CreateDelegate(adapterType, factory.Target, factory.Method);
        }
        catch (ArgumentException)
        {
            var spParam = Expression.Parameter(typeof(IServiceProvider), "sp");
            var invokeExpr = Expression.Invoke(Expression.Constant(factory), spParam);
            var castExpr = Expression.Convert(invokeExpr, viewType);
            adapter = Expression.Lambda(adapterType, castExpr, spParam).Compile();
        }

        var wrapper = (IViewWrapper)Activator.CreateInstance(wrapperType, adapter)!;
        return Create(viewType, services, viewLifetime, wrapper);
    }

    internal static ViewBuilder Create(
        Type viewType,
        IServiceCollection services,
        ServiceLifetime viewLifetime,
        Type implType
    )
    {
        var wrapperType = typeof(TypeViewWrapper<>).MakeGenericType(viewType);
        var wrapper = (IViewWrapper)Activator.CreateInstance(wrapperType, implType)!;
        return Create(viewType, services, viewLifetime, wrapper);
    }

    public ViewBuilder WithViewModel(Type viewModelType, bool isDefault = false)
    {
        _services.AddSingleton(
            typeof(IViewModelConfiguration<>).MakeGenericType(viewModelType),
            new ViewModelConfiguration(_viewType)
        );
        _viewWrapper?.WithViewModelType(viewModelType, isDefault);
        return this;
    }

    public ViewBuilder WithName(string name)
    {
        _services.AddKeyedSingleton<IViewNameConfiguration>(name, new ViewNameConfiguration(_viewType));
        return this;
    }

    internal interface IViewWrapper
    {
        Type? DefaultViewModelType { get; }
        FrameworkElement GetInstance(IServiceProvider serviceProvider);
        void WithViewModelType(Type viewModelType, bool isDefault = false);
        IViewWrapper Clone();
    }

    internal interface IViewWrapper<out TView> : IViewWrapper
        where TView : FrameworkElement;

    private abstract class ViewWrapper<TView> : IViewWrapper<TView>
        where TView : FrameworkElement
    {
#if NET9_0_OR_GREATER
        private readonly Lock _lock = new();
#else
        private readonly object _lock = new();
#endif
        private TView? _instance;

        Type? IViewWrapper.DefaultViewModelType => _defaultViewModelType ?? _lastViewModelType;

        protected Type? _defaultViewModelType;
        protected Type? _lastViewModelType;

        void IViewWrapper.WithViewModelType(Type viewModelType, bool isDefault)
        {
            if (isDefault)
            {
                _defaultViewModelType = viewModelType;
            }
            else
            {
                _lastViewModelType = viewModelType;
            }
        }

        FrameworkElement IViewWrapper.GetInstance(IServiceProvider serviceProvider)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= CreateInstance(serviceProvider);
                }
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
            if (Application.Current?.Dispatcher is not { } dispatcher || dispatcher.CheckAccess())
            {
                return (TView)_factory(serviceProvider, []);
            }

            return (TView)dispatcher.Invoke(() => _factory(serviceProvider, []));
        }

        protected override ViewWrapper<TView> Clone()
        {
            return new TypeViewWrapper<TView>(_implType, _factory)
            {
                _defaultViewModelType = _defaultViewModelType, _lastViewModelType = _lastViewModelType,
            };
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
            return new FactoryViewWrapper<TView>(factory)
            {
                _defaultViewModelType = _defaultViewModelType, _lastViewModelType = _lastViewModelType,
            };
        }
    }

    private class InstanceViewWrapper<TView>(TView instance) : IViewWrapper<TView>
        where TView : FrameworkElement
    {
        private Type? _defaultViewModelType;
        private Type? _lastViewModelType;
        Type? IViewWrapper.DefaultViewModelType => _defaultViewModelType ?? _lastViewModelType;

        void IViewWrapper.WithViewModelType(Type viewModelType, bool isDefault)
        {
            if (isDefault)
            {
                _defaultViewModelType = viewModelType;
            }
            else
            {
                _lastViewModelType = viewModelType;
            }
        }

        FrameworkElement IViewWrapper.GetInstance(IServiceProvider serviceProvider)
        {
            return instance;
        }

        IViewWrapper IViewWrapper.Clone()
        {
            return new InstanceViewWrapper<TView>(instance)
            {
                _defaultViewModelType = _defaultViewModelType, _lastViewModelType = _lastViewModelType,
            };
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
