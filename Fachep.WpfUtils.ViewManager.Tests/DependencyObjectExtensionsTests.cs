using System.Windows;
using System.Windows.Controls;
using Fachep.WpfUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils.Tests;

[STATestClass]
public sealed class DependencyObjectExtensionsTests
{
    private static IServiceProvider BuildServiceProviderWithViewManagerInResources(
        Action<IServiceCollection>? configureServices = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ViewManager>();
        configureServices?.Invoke(services);
        var sp = services.BuildServiceProvider();
        return sp;
    }

    private static FrameworkElement CreateElementWithViewManager(IServiceProvider sp)
    {
        var manager = sp.GetRequiredService<ViewManager>();
        var element = new UserControl();
        element.Resources[ViewManager.ViewManagerResourceKey] = manager;
        return element;
    }

    [TestMethod]
    public void ViewManager_FromFrameworkElement_ReturnsViewManager()
    {
        var sp = BuildServiceProviderWithViewManagerInResources();
        var element = CreateElementWithViewManager(sp);

        var vm = element.ViewManager;
        Assert.IsNotNull(vm);
    }

    [TestMethod]
    public void GetView_ByType_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var view = element.GetView(typeof(TestView));
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestView>(view);
    }

    [TestMethod]
    public void GetView_Generic_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var view = element.GetView<TestView>();
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void GetView_GenericWithViewModel_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var view = element.GetView<TestView, TestViewModel>();
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetViewByViewModel_ByType_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var view = element.GetViewByViewModel(typeof(TestViewModel));
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void GetViewByViewModel_Generic_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var view = element.GetViewByViewModel<TestViewModel>();
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void GetViewByViewModel_GenericWithTView_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var view = element.GetViewByViewModel<TestViewModel, TestView>();
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void GetView_ByName_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("Named");
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var view = element.GetView("Named");
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void GetView_GenericByName_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("Named");
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var view = element.GetView<TestView>("Named");
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void GetView_GenericViewAndVm_ByName_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("Named")
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var view = element.GetView<TestView, TestViewModel>("Named");
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetView_WithObjectViewModel_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var vm = new TestViewModel();
        var view = element.GetView(typeof(TestView), (object)vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetView_GenericWithObjectViewModel_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var vm = new TestViewModel();
        var view = element.GetView<TestView>((object)vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetViewByViewModel_WithObjectVm_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var vm = new TestViewModel();
        var view = element.GetViewByViewModel(typeof(TestViewModel), (object)vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetViewByViewModel_GenericWithInstance_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var vm = new TestViewModel();
        var view = element.GetViewByViewModel<TestViewModel>(vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetViewByViewModel_GenericWithTViewAndInstance_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var vm = new TestViewModel();
        var view = element.GetViewByViewModel<TestViewModel, TestView>(vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetView_ByName_WithObjectViewModel_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("Named");
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var vm = new TestViewModel();
        var view = element.GetView("Named", (object)vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetView_GenericByName_WithObjectViewModel_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("Named");
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        var vm = new TestViewModel();
        var view = element.GetView<TestView>("Named", (object)vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetView_ByType_WithCallback_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        object? captured = null;
        var view = element.GetView(typeof(TestView), (Action<object>)(vm => captured = vm));
        Assert.IsNotNull(view);
        Assert.IsNotNull(captured);
    }

    [TestMethod]
    public void GetView_Generic_WithCallback_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        object? captured = null;
        var view = element.GetView<TestView>((Action<object>)(vm => captured = vm));
        Assert.IsNotNull(view);
        Assert.IsNotNull(captured);
    }

    [TestMethod]
    public void GetView_ByName_WithCallback_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("Named")
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        object? captured = null;
        var view = element.GetView("Named", (Action<object>)(vm => captured = vm));
        Assert.IsNotNull(view);
        Assert.IsNotNull(captured);
    }

    [TestMethod]
    public void GetView_GenericByName_WithCallback_DelegatesToViewManager()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("Named")
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var element = CreateElementWithViewManager(sp);

        object? captured = null;
        var view = element.GetView<TestView>("Named", (Action<object>)(vm => captured = vm));
        Assert.IsNotNull(view);
        Assert.IsNotNull(captured);
    }
}
