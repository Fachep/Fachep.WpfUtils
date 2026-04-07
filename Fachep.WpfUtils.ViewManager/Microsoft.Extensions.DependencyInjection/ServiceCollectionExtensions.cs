using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Fachep.WpfUtils;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public ViewBuilder AddView(Type viewType, ServiceLifetime viewLifetime, Type? implType = null)
        {
            return ViewBuilder.Create(viewType, services, viewLifetime, implType ?? viewType);
        }

        public ViewBuilder AddView<TView>(ServiceLifetime viewLifetime)
            where TView : FrameworkElement
        {
            return ViewBuilder.Create(typeof(TView), services, viewLifetime, typeof(TView));
        }

        public ViewBuilder AddView<TView, TViewImpl>(ServiceLifetime viewLifetime)
            where TView : FrameworkElement
            where TViewImpl : TView
        {
            return ViewBuilder.Create(typeof(TView), services, viewLifetime, typeof(TViewImpl));
        }

        public ViewBuilder AddSingletonView(Type viewType, Type? implType = null)
        {
            return ViewBuilder.Create(viewType, services, ServiceLifetime.Singleton, implType ?? viewType);
        }

        public ViewBuilder AddSingletonView<TView>()
            where TView : FrameworkElement
        {
            return ViewBuilder.Create(typeof(TView), services, ServiceLifetime.Singleton, typeof(TView));
        }

        public ViewBuilder AddSingletonView<TView, TViewImpl>()
            where TView : FrameworkElement
            where TViewImpl : TView
        {
            return ViewBuilder.Create(typeof(TView), services, ServiceLifetime.Singleton, typeof(TViewImpl));
        }

        public ViewBuilder AddScopedView(Type viewType, Type? implType = null)
        {
            return ViewBuilder.Create(viewType, services, ServiceLifetime.Scoped, implType ?? viewType);
        }

        public ViewBuilder AddScopedView<TView>()
            where TView : FrameworkElement
        {
            return ViewBuilder.Create(typeof(TView), services, ServiceLifetime.Scoped, typeof(TView));
        }

        public ViewBuilder AddScopedView<TView, TViewImpl>()
            where TView : FrameworkElement
            where TViewImpl : TView
        {
            return ViewBuilder.Create(typeof(TView), services, ServiceLifetime.Scoped, typeof(TViewImpl));
        }

        public ViewBuilder AddTransientView(Type viewType, Type? implType = null)
        {
            return ViewBuilder.Create(viewType, services, ServiceLifetime.Transient, implType ?? viewType);
        }

        public ViewBuilder AddTransientView<TView>()
            where TView : FrameworkElement
        {
            return ViewBuilder.Create(typeof(TView), services, ServiceLifetime.Transient, typeof(TView));
        }

        public ViewBuilder AddTransientView<TView, TViewImpl>()
            where TView : FrameworkElement
            where TViewImpl : TView
        {
            return ViewBuilder.Create(typeof(TView), services, ServiceLifetime.Transient, typeof(TViewImpl));
        }

        public ViewBuilder AddView(
            Type viewType,
            ServiceLifetime viewLifetime,
            Func<IServiceProvider, FrameworkElement> factory
        )
        {
            return ViewBuilder.Create(viewType, services, viewLifetime, factory);
        }

        public ViewBuilder AddView<TView>(ServiceLifetime viewLifetime, Func<IServiceProvider, TView> factory)
            where TView : FrameworkElement
        {
            return ViewBuilder.Create(typeof(TView), services, viewLifetime, factory);
        }

        public ViewBuilder AddSingletonView(Type viewType, Func<IServiceProvider, FrameworkElement> factory)
        {
            return ViewBuilder.Create(viewType, services, ServiceLifetime.Singleton, factory);
        }

        public ViewBuilder AddSingletonView<TView>(Func<IServiceProvider, TView> factory)
            where TView : FrameworkElement
        {
            return ViewBuilder.Create(typeof(TView), services, ServiceLifetime.Singleton, factory);
        }

        public ViewBuilder AddScopedView(Type viewType, Func<IServiceProvider, FrameworkElement> factory)
        {
            return ViewBuilder.Create(viewType, services, ServiceLifetime.Scoped, factory);
        }

        public ViewBuilder AddScopedView<TView>(Func<IServiceProvider, TView> factory)
            where TView : FrameworkElement
        {
            return ViewBuilder.Create(typeof(TView), services, ServiceLifetime.Scoped, factory);
        }

        public ViewBuilder AddTransientView(Type viewType, Func<IServiceProvider, FrameworkElement> factory)
        {
            return ViewBuilder.Create(viewType, services, ServiceLifetime.Transient, factory);
        }

        public ViewBuilder AddTransientView<TView>(Func<IServiceProvider, TView> factory)
            where TView : FrameworkElement
        {
            return ViewBuilder.Create(typeof(TView), services, ServiceLifetime.Transient, factory);
        }

        public ViewBuilder AddView(Type viewType, FrameworkElement instance)
        {
            return ViewBuilder.Create(viewType, services, instance);
        }

        public ViewBuilder AddView<TView>(TView instance)
            where TView : FrameworkElement
        {
            return ViewBuilder.Create(typeof(TView), services, instance);
        }

        public ViewBuilder ConfigureView(Type viewType)
        {
            return ViewBuilder.Create(viewType, services);
        }

        public ViewBuilder ConfigureView<TView>()
        {
            return ViewBuilder.Create(typeof(TView), services);
        }

        public IServiceCollection AddViews(Assembly? assembly = null, Type? appType = null)
        {
            if (appType is not null && !typeof(Application).IsAssignableFrom(appType))
            {
                throw new ArgumentException("appType must be an Application.", nameof(appType));
            }

            assembly ??= Assembly.GetCallingAssembly();
            var viewModels = new Dictionary<string, Type>();
            var viewWithoutViewModels = new List<Type>();
            foreach (var type in assembly.GetTypes())
            {
                if (type is not Type { IsPublic: true, IsAbstract: false, IsGenericTypeDefinition: false })
                {
                    continue;
                }

                if (appType is null && typeof(Application).IsAssignableFrom(type))
                {
                    appType = type;
                    continue;
                }

                if (type.Name.EndsWith("ViewModel"))
                {
                    if (!viewModels.Remove(type.Name))
                    {
                        viewModels[type.Name] = type;
                    }
                }

                if (!typeof(FrameworkElement).IsAssignableFrom(type))
                {
                    continue;
                }

                var viewAttr = type.GetCustomAttribute<ViewAttribute>();
                if (viewAttr is null)
                {
                    if ((typeof(Page).IsAssignableFrom(type) || typeof(Window).IsAssignableFrom(type)) &&
                        (type.Name.EndsWith("View") || type.Name.EndsWith("Page") || type.Name.EndsWith("Window")))
                    {
                        viewAttr = new ViewAttribute(type.Name);
                    }
                }

                if (viewAttr is null)
                {
                    continue;
                }

                var viewModelAttrs = type.GetCustomAttributes<WithViewModelAttribute>();
                var builder = services.AddView(type, viewAttr.LifeTime, viewAttr.ViewType ?? type);
                if (viewAttr.Names.Length > 0)
                {
                    foreach (var name in viewAttr.Names)
                    {
                        builder.WithName(name);
                    }
                }
                else
                {
                    builder.WithName(type.Name);
                }

                var hasViewModel = false;
                foreach (var viewModelAttr in viewModelAttrs)
                {
                    hasViewModel = true;
                    builder.WithViewModel(viewModelAttr.ViewModelType, viewModelAttr.IsDefault);
                    services.Add(
                        new ServiceDescriptor(
                            viewModelAttr.ViewModelType, viewModelAttr.ViewModelType,
                            viewModelAttr.LifeTime
                        )
                    );
                }

                if (!hasViewModel)
                {
                    viewWithoutViewModels.Add(type);
                }
            }

            foreach (var viewType in viewWithoutViewModels)
            {
                var viewName = viewType.Name;
                if (viewName.EndsWith("View"))
                {
#if NETCOREAPP
                    viewName = viewName[..^"View".Length];
#else
                    viewName = viewName.Substring(0, viewName.Length - "View".Length);
#endif
                }
                else if (viewName.EndsWith("Page"))
                {
#if NETCOREAPP
                    viewName = viewName[..^"Page".Length];
#else
                    viewName = viewName.Substring(0, viewName.Length - "Page".Length);
#endif
                }
                else if (viewName.EndsWith("Window"))
                {
#if NETCOREAPP
                    viewName = viewName[..^"Window".Length];
#else
                    viewName = viewName.Substring(0, viewName.Length - "Window".Length);
#endif
                }

                if (viewModels.TryGetValue($"{viewName}ViewModel", out var viewModelType))
                {
                    services.ConfigureView(viewType)
                        .WithViewModel(viewModelType);
                }
            }

            if (appType is not null)
            {
                services.AddSingleton(
                    appType, sp =>
                    {
                        var app = (Application)ActivatorUtilities.CreateInstance(sp, appType);
                        app.Resources[ViewManager.ViewManagerResourceKey] = sp.GetRequiredService<ViewManager>();
                        return app;
                    }
                );
                services.AddSingleton<Application>(sp => (Application)sp.GetRequiredService(appType));
            }

            return services;
        }

        public IServiceCollection AddViews<TApp>(Assembly? assembly = null)
            where TApp : Application
        {
            return services.AddViews(assembly, typeof(TApp));
        }
    }
}
