using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils.Tests;

[STATestClass]
public sealed class ServiceCollectionExtensionsTests
{
    // ===== AddView with lifetime (factory-based to avoid Application.Current dependency) =====

    [TestMethod]
    public void AddView_ByType_WithFactory_RegistersView()
    {
        var services = new ServiceCollection();
        services.AddView(
            typeof(TestView), ServiceLifetime.Singleton,
            sp => new TestView()
        );
        var sp = services.BuildServiceProvider();

        var view = sp.GetService(typeof(TestView));
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestView>(view);
    }

    [TestMethod]
    public void AddView_Generic_WithFactory_RegistersView()
    {
        var services = new ServiceCollection();
        services.AddView<TestView>(ServiceLifetime.Singleton, sp => new TestView());
        var sp = services.BuildServiceProvider();

        var view = sp.GetService<TestView>();
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void AddView_ByType_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddView(typeof(TestView), ServiceLifetime.Singleton);
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddView_ByType_WithImplType_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddView(typeof(TestViewBase), ServiceLifetime.Singleton, typeof(CustomViewTypeView));
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddView_Generic_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddView<TestView>(ServiceLifetime.Singleton);
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddView_GenericWithImpl_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddView<TestViewBase, CustomViewTypeView>(ServiceLifetime.Singleton);
        Assert.IsNotNull(builder);
    }

    // ===== AddSingletonView =====

    [TestMethod]
    public void AddSingletonView_ByType_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddSingletonView(typeof(TestView));
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddSingletonView_ByType_WithImplType_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddSingletonView(typeof(TestViewBase), typeof(CustomViewTypeView));
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddSingletonView_GenericWithFactory_ReturnsSameInstance()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();

        var v1 = sp.GetService<TestView>();
        var v2 = sp.GetService<TestView>();
        Assert.AreSame(v1, v2);
    }

    [TestMethod]
    public void AddSingletonView_GenericWithImpl_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddSingletonView<TestViewBase, CustomViewTypeView>();
        Assert.IsNotNull(builder);
    }

    // ===== AddScopedView =====

    [TestMethod]
    public void AddScopedView_ByType_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddScopedView(typeof(TestView));
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddScopedView_ByType_WithImplType_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddScopedView(typeof(TestViewBase), typeof(CustomViewTypeView));
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddScopedView_GenericWithFactory_DifferentScopes_ReturnsDifferentInstances()
    {
        var services = new ServiceCollection();
        services.AddScopedView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();

        TestView? v1, v2;
        using (var scope1 = sp.CreateScope())
        {
            v1 = scope1.ServiceProvider.GetService<TestView>();
        }

        using (var scope2 = sp.CreateScope())
        {
            v2 = scope2.ServiceProvider.GetService<TestView>();
        }

        Assert.IsNotNull(v1);
        Assert.IsNotNull(v2);
        Assert.AreNotSame(v1, v2);
    }

    [TestMethod]
    public void AddScopedView_Generic_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddScopedView<TestView>();
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddScopedView_GenericWithImpl_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddScopedView<TestViewBase, CustomViewTypeView>();
        Assert.IsNotNull(builder);
    }

    // ===== AddTransientView =====

    [TestMethod]
    public void AddTransientView_ByType_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddTransientView(typeof(TestView));
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddTransientView_ByType_WithImplType_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddTransientView(typeof(TestViewBase), typeof(CustomViewTypeView));
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddTransientView_GenericWithFactory_ReturnsDifferentInstances()
    {
        var services = new ServiceCollection();
        services.AddTransientView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();

        var v1 = sp.GetService<TestView>();
        var v2 = sp.GetService<TestView>();
        Assert.IsNotNull(v1);
        Assert.IsNotNull(v2);
        Assert.AreNotSame(v1, v2);
    }

    [TestMethod]
    public void AddTransientView_Generic_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddTransientView<TestView>();
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void AddTransientView_GenericWithImpl_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddTransientView<TestViewBase, CustomViewTypeView>();
        Assert.IsNotNull(builder);
    }

    // ===== AddView with factory =====

    [TestMethod]
    public void AddView_WithFactory_RegistersView()
    {
        var services = new ServiceCollection();
        var expected = new TestView();
        services.AddView(
            typeof(TestView), ServiceLifetime.Singleton,
            sp => expected
        );
        var sp = services.BuildServiceProvider();

        var view = sp.GetService(typeof(TestView));
        Assert.AreSame(expected, view);
    }

    [TestMethod]
    public void AddView_GenericWithFactory_RegistersView()
    {
        var services = new ServiceCollection();
        var expected = new TestView();
        services.AddView<TestView>(ServiceLifetime.Singleton, sp => expected);
        var sp = services.BuildServiceProvider();

        var view = sp.GetService<TestView>();
        Assert.AreSame(expected, view);
    }

    [TestMethod]
    public void AddSingletonView_WithFactory_RegistersView()
    {
        var services = new ServiceCollection();
        var expected = new TestView();
        services.AddSingletonView(
            typeof(TestView),
            sp => expected
        );
        var sp = services.BuildServiceProvider();

        var view = sp.GetService(typeof(TestView));
        Assert.AreSame(expected, view);
    }

    [TestMethod]
    public void AddSingletonView_GenericWithFactory_RegistersView()
    {
        var services = new ServiceCollection();
        var expected = new TestView();
        services.AddSingletonView<TestView>(sp => expected);
        var sp = services.BuildServiceProvider();

        var view = sp.GetService<TestView>();
        Assert.AreSame(expected, view);
    }

    [TestMethod]
    public void AddScopedView_WithFactory_RegistersView()
    {
        var services = new ServiceCollection();
        services.AddScopedView(
            typeof(TestView),
            sp => new TestView()
        );
        var sp = services.BuildServiceProvider();

        using var scope = sp.CreateScope();
        var view = scope.ServiceProvider.GetService(typeof(TestView));
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void AddScopedView_GenericWithFactory_RegistersView()
    {
        var services = new ServiceCollection();
        services.AddScopedView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();

        using var scope = sp.CreateScope();
        var view = scope.ServiceProvider.GetService<TestView>();
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void AddTransientView_WithFactory_RegistersView()
    {
        var services = new ServiceCollection();
        services.AddTransientView(
            typeof(TestView),
            sp => new TestView()
        );
        var sp = services.BuildServiceProvider();

        var v1 = sp.GetService(typeof(TestView));
        var v2 = sp.GetService(typeof(TestView));
        Assert.AreNotSame(v1, v2);
    }

    [TestMethod]
    public void AddTransientView_GenericWithFactory_RegistersView()
    {
        var services = new ServiceCollection();
        services.AddTransientView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();

        var v1 = sp.GetService<TestView>();
        var v2 = sp.GetService<TestView>();
        Assert.AreNotSame(v1, v2);
    }

    // ===== AddView with instance =====

    [TestMethod]
    public void AddView_WithInstance_RegistersSameInstance()
    {
        var services = new ServiceCollection();
        var instance = new TestView();
        services.AddView(typeof(TestView), instance);
        var sp = services.BuildServiceProvider();

        var view = sp.GetService(typeof(TestView));
        Assert.AreSame(instance, view);
    }

    [TestMethod]
    public void AddView_GenericWithInstance_RegistersSameInstance()
    {
        var services = new ServiceCollection();
        var instance = new TestView();
        services.AddView(instance);
        var sp = services.BuildServiceProvider();

        var view = sp.GetService<TestView>();
        Assert.AreSame(instance, view);
    }

    // ===== ConfigureView =====

    [TestMethod]
    public void ConfigureView_ByType_RegistersViewManagerAndReturnsBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.ConfigureView(typeof(TestView));
        Assert.IsNotNull(builder);

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<ViewManager>());
    }

    [TestMethod]
    public void ConfigureView_Generic_RegistersViewManagerAndReturnsBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.ConfigureView<TestView>();
        Assert.IsNotNull(builder);

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<ViewManager>());
    }

    [TestMethod]
    public void ConfigureView_CanAddViewModelAndName()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        services.ConfigureView<TestView>()
            .WithViewModel(typeof(TestViewModel))
            .WithName("Configured");
        services.AddSingleton<TestViewModel>();

        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("Configured");
        Assert.IsNotNull(view);
    }

    // ===== AddViews (auto-discovery) =====
    // Note: AddViews uses TypeViewWrapper which requires Application.Current.Dispatcher.
    // Tests that resolve views are skipped. Tests verify registration/configuration only.

    [TestMethod]
    public void AddViews_RegistersViewManager()
    {
        var services = new ServiceCollection();
        services.AddViews(typeof(AttributedView).Assembly);
        var sp = services.BuildServiceProvider();

        Assert.IsNotNull(sp.GetService<ViewManager>());
    }

    [TestMethod]
    public void AddViews_RegistersNameConfiguration_ForAttributedView()
    {
        var services = new ServiceCollection();
        services.AddViews(typeof(AttributedView).Assembly);
        var sp = services.BuildServiceProvider();

        var config = sp.GetKeyedService<ViewBuilder.IViewNameConfiguration>("MyView");
        Assert.IsNotNull(config);
        Assert.AreEqual(typeof(AttributedView), config.ViewType);
    }

    [TestMethod]
    public void AddViews_RegistersMultipleNameConfigurations()
    {
        var services = new ServiceCollection();
        services.AddViews(typeof(MultiNamedView).Assembly);
        var sp = services.BuildServiceProvider();

        var c1 = sp.GetKeyedService<ViewBuilder.IViewNameConfiguration>("NamedView1");
        var c2 = sp.GetKeyedService<ViewBuilder.IViewNameConfiguration>("NamedView2");
        Assert.IsNotNull(c1);
        Assert.IsNotNull(c2);
        Assert.AreEqual(typeof(MultiNamedView), c1.ViewType);
        Assert.AreEqual(typeof(MultiNamedView), c2.ViewType);
    }

    [TestMethod]
    public void AddViews_RegistersViewModelConfiguration_ForViewWithViewModel()
    {
        var services = new ServiceCollection();
        services.AddViews(typeof(ViewWithViewModel).Assembly);
        var sp = services.BuildServiceProvider();

        var configType = typeof(ViewBuilder.IViewModelConfiguration<>).MakeGenericType(typeof(TestViewModel));
        var config = sp.GetService(configType) as ViewBuilder.IViewModelConfiguration<object>;
        Assert.IsNotNull(config);
    }

    [TestMethod]
    public void AddViews_RegistersMultipleViewModels_DefaultTakesPrecedence()
    {
        var services = new ServiceCollection();
        services.AddViews(typeof(ViewWithMultipleViewModels).Assembly);
        var sp = services.BuildServiceProvider();

        // Verify that the wrapper has TestViewModel2 as default
        var wrapperType = typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(typeof(ViewWithMultipleViewModels));
        var wrapper = sp.GetService(wrapperType) as ViewBuilder.IViewWrapper;
        Assert.IsNotNull(wrapper);
        Assert.AreEqual(typeof(TestViewModel2), wrapper.DefaultViewModelType);
    }

    [TestMethod]
    public void AddViews_RegistersViewModelService()
    {
        var services = new ServiceCollection();
        services.AddViews(typeof(ViewWithViewModel).Assembly);
        var sp = services.BuildServiceProvider();

        // TestViewModel should be registered in the container
        var vm = sp.GetService<TestViewModel>();
        Assert.IsNotNull(vm);
    }

    [TestMethod]
    public void AddViews_ConventionBasedViewModel_RegistersViewModelConfiguration()
    {
        var services = new ServiceCollection();
        services.AddViews(typeof(OrderView).Assembly);
        var sp = services.BuildServiceProvider();

        // OrderView should be matched with OrderViewModel by naming convention
        var configType = typeof(ViewBuilder.IViewModelConfiguration<>).MakeGenericType(typeof(OrderViewModel));
        var config = sp.GetService(configType) as ViewBuilder.IViewModelConfiguration<object>;
        Assert.IsNotNull(config);
        Assert.AreEqual(typeof(OrderView), config.ViewType);
    }

    [TestMethod]
    public void AddViews_InvalidAppType_ThrowsArgumentException()
    {
        var services = new ServiceCollection();
        Assert.ThrowsExactly<ArgumentException>(() =>
            services.AddViews(typeof(AttributedView).Assembly, typeof(TestView))
        );
    }

    [TestMethod]
    public void AddViews_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddViews(typeof(AttributedView).Assembly);
        Assert.AreSame(services, result);
    }

    // ===== AddView registers ViewManager =====

    [TestMethod]
    public void AddView_AlwaysRegistersViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();

        Assert.IsNotNull(sp.GetService<ViewManager>());
    }

    // ===== Multiple views can be registered =====

    [TestMethod]
    public void MultipleViews_CanBeRegistered()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView()).WithName("View1");
        services.AddSingletonView<TestView2>(sp => new TestView2()).WithName("View2");
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var v1 = manager.GetView("View1");
        var v2 = manager.GetView("View2");
        Assert.IsNotNull(v1);
        Assert.IsNotNull(v2);
        Assert.IsInstanceOfType<TestView>(v1);
        Assert.IsInstanceOfType<TestView2>(v2);
    }

    // ===== Chaining =====

    [TestMethod]
    public void FluentChaining_AddView_WithViewModel_WithName()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel))
            .WithName("ChainedView");
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("ChainedView");
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }
}
