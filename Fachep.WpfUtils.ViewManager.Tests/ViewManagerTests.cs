using System.Windows;
using System.Windows.Controls;
using Fachep.WpfUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils.Tests;

[STATestClass]
public sealed class ViewManagerTests
{
    // ===== GetView by Type =====

    [TestMethod]
    public void GetView_ByType_ReturnsView()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView(typeof(TestView));
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestView>(view);
    }

    [TestMethod]
    public void GetView_ByType_UnregisteredType_ReturnsNull()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ViewManager>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView(typeof(TestView));
        Assert.IsNull(view);
    }

    [TestMethod]
    public void GetView_ByType_WithViewModelType_SetsDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView(typeof(TestView), typeof(TestViewModel));
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetView_ByType_WithViewModelCallback_InvokesCallback()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        object? callbackVm = null;
        var view = manager.GetView(typeof(TestView), typeof(TestViewModel), vm => callbackVm = vm);
        Assert.IsNotNull(callbackVm);
        Assert.IsInstanceOfType<TestViewModel>(callbackVm);
    }

    [TestMethod]
    public void GetView_ByType_WithoutExplicitVmType_UsesDefaultViewModelType()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView(typeof(TestView));
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetView_ByType_ViewModelNotRegistered_DataContextNotSet()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        // Deliberately not registering TestViewModel
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView(typeof(TestView), typeof(TestViewModel));
        Assert.IsNotNull(view);
        Assert.IsNull(view.DataContext);
    }

    [TestMethod]
    public void GetView_ByType_CallbackOverload_InvokesCallback()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        object? captured = null;
        var view = manager.GetView(typeof(TestView), (Action<object>)(vm => captured = vm));
        Assert.IsNotNull(view);
        Assert.IsNotNull(captured);
    }

    // ===== GetView<TView> =====

    [TestMethod]
    public void GetView_Generic_ReturnsTypedView()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView<TestView>();
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void GetView_Generic_UnregisteredType_ReturnsNull()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ViewManager>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView<TestView>();
        Assert.IsNull(view);
    }

    [TestMethod]
    public void GetView_Generic_WithViewModelType_SetsDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView<TestView>(viewModelType: typeof(TestViewModel));
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetView_Generic_CallbackOverload_InvokesCallback()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        object? captured = null;
        var view = manager.GetView<TestView>((Action<object>)(vm => captured = vm));
        Assert.IsNotNull(view);
        Assert.IsNotNull(captured);
    }

    // ===== GetView<TView, TViewModel> =====

    [TestMethod]
    public void GetView_GenericViewAndViewModel_ReturnsViewWithDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView<TestView, TestViewModel>();
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetView_GenericViewAndViewModel_WithCallback_InvokesTypedCallback()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        TestViewModel? captured = null;
        var view = manager.GetView<TestView, TestViewModel>(vm => captured = vm);
        Assert.IsNotNull(captured);
    }

    [TestMethod]
    public void GetView_GenericViewAndViewModel_NullCallback_DoesNotThrow()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView<TestView, TestViewModel>((Action<TestViewModel>?)null);
        Assert.IsNotNull(view);
    }

    // ===== GetView with object viewModel =====

    [TestMethod]
    public void GetView_WithObjectViewModel_SetsDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var vm = new TestViewModel();
        var view = manager.GetView(typeof(TestView), (object)vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetView_WithTypeAsObjectViewModel_TreatsAsViewModelType()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        // When viewModel is a Type, it should treat it as viewModelType
        var view = manager.GetView(typeof(TestView), (object)typeof(TestViewModel));
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetView_WithObjectViewModel_UnregisteredView_ReturnsNull()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ViewManager>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView(typeof(TestView), (object)new TestViewModel());
        Assert.IsNull(view);
    }

    [TestMethod]
    public void GetView_GenericWithObjectViewModel_ReturnsTypedView()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var vm = new TestViewModel();
        var view = manager.GetView<TestView>((object)vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    // ===== GetViewByViewModel =====

    [TestMethod]
    public void GetViewByViewModel_ByType_ReturnsViewWithDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetViewByViewModel(typeof(TestViewModel));
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetViewByViewModel_ByType_UnconfiguredViewModel_ReturnsNull()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetViewByViewModel(typeof(TestViewModel));
        Assert.IsNull(view);
    }

    [TestMethod]
    public void GetViewByViewModel_ByType_WithCallback_InvokesCallback()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        object? captured = null;
        manager.GetViewByViewModel(typeof(TestViewModel), vm => captured = vm);
        Assert.IsNotNull(captured);
    }

    [TestMethod]
    public void GetViewByViewModel_Generic_ReturnsView()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetViewByViewModel<TestViewModel>();
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void GetViewByViewModel_GenericWithCallback_InvokesTypedCallback()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        TestViewModel? captured = null;
        manager.GetViewByViewModel<TestViewModel>(vm => captured = vm);
        Assert.IsNotNull(captured);
    }

    [TestMethod]
    public void GetViewByViewModel_GenericWithTView_ReturnsTypedView()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetViewByViewModel<TestViewModel, TestView>();
        Assert.IsNotNull(view);
    }

    // ===== GetViewByViewModel with object viewModel =====

    [TestMethod]
    public void GetViewByViewModel_WithObjectViewModel_SetsDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var vm = new TestViewModel();
        var view = manager.GetViewByViewModel(typeof(TestViewModel), (object)vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetViewByViewModel_WithObjectViewModel_UnconfiguredVm_ReturnsNull()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView());
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetViewByViewModel(typeof(TestViewModel), (object)new TestViewModel());
        Assert.IsNull(view);
    }

    [TestMethod]
    public void GetViewByViewModel_GenericWithInstance_SetsDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var vm = new TestViewModel();
        var view = manager.GetViewByViewModel<TestViewModel>(vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetViewByViewModel_GenericWithTypeInstance_TreatsAsViewModelType()
    {
        // When TViewModel is object and value is Type, should resolve by type
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        // This calls GetViewByViewModel<Type>(typeof(TestViewModel))
        // which triggers the `is Type` branch
        var view = manager.GetViewByViewModel<Type>(typeof(TestViewModel));
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void GetViewByViewModel_GenericWithTViewAndInstance_ReturnsTypedView()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithViewModel(typeof(TestViewModel));
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var vm = new TestViewModel();
        var view = manager.GetViewByViewModel<TestViewModel, TestView>(vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    // ===== GetView by Name =====

    [TestMethod]
    public void GetView_ByName_ReturnsView()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView");
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("MyView");
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestView>(view);
    }

    [TestMethod]
    public void GetView_ByName_UnregisteredName_ReturnsNull()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ViewManager>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("NonExistent");
        Assert.IsNull(view);
    }

    [TestMethod]
    public void GetView_ByName_WithViewModelType_SetsDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView")
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("MyView", typeof(TestViewModel));
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetView_ByName_WithDefaultViewModelType_SetsDataContextAutomatically()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView")
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("MyView");
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetView_ByName_KeyedViewModel_ResolvesKeyedService()
    {
        var services = new ServiceCollection();
        var keyedVm = new TestViewModel();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView")
            .WithViewModel(typeof(TestViewModel));
        services.AddKeyedSingleton<TestViewModel>("MyView", keyedVm);
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("MyView", typeof(TestViewModel), keyedViewModel: true);
        Assert.IsNotNull(view);
        Assert.AreSame(keyedVm, view.DataContext);
    }

    [TestMethod]
    public void GetView_ByName_WithCallback_InvokesCallback()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView")
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        object? captured = null;
        manager.GetView("MyView", typeof(TestViewModel), false, vm => captured = vm);
        Assert.IsNotNull(captured);
    }

    [TestMethod]
    public void GetView_ByName_CallbackOverload_InvokesCallback()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView")
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        object? captured = null;
        manager.GetView("MyView", (Action<object>)(vm => captured = vm));
        Assert.IsNotNull(captured);
    }

    [TestMethod]
    public void GetView_ByName_NoVmType_NoDefault_ReturnsViewWithoutDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView");
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("MyView");
        Assert.IsNotNull(view);
        Assert.IsNull(view.DataContext);
    }

    [TestMethod]
    public void GetView_ByName_VmNotRegistered_ViewReturned_DataContextNull()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView")
            .WithViewModel(typeof(TestViewModel));
        // TestViewModel not registered
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("MyView", typeof(TestViewModel));
        Assert.IsNotNull(view);
        Assert.IsNull(view.DataContext);
    }

    // ===== GetView<TView> by Name =====

    [TestMethod]
    public void GetView_GenericByName_ReturnsTypedView()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView");
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView<TestView>("MyView");
        Assert.IsNotNull(view);
    }

    [TestMethod]
    public void GetView_GenericByName_CallbackOverload_InvokesCallback()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView")
            .WithViewModel(typeof(TestViewModel), isDefault: true);
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        object? captured = null;
        var view = manager.GetView<TestView>("MyView", (Action<object>)(vm => captured = vm));
        Assert.IsNotNull(view);
        Assert.IsNotNull(captured);
    }

    // ===== GetView<TView, TViewModel> by Name =====

    [TestMethod]
    public void GetView_GenericViewAndVm_ByName_ReturnsViewWithDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView")
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView<TestView, TestViewModel>("MyView");
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetView_GenericViewAndVm_ByName_WithCallback_InvokesTypedCallback()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView")
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        TestViewModel? captured = null;
        var view = manager.GetView<TestView, TestViewModel>("MyView", viewModelCallback: vm => captured = vm);
        Assert.IsNotNull(captured);
    }

    // ===== GetView by Name with object viewModel =====

    [TestMethod]
    public void GetView_ByName_WithObjectViewModel_SetsDataContext()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView");
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var vm = new TestViewModel();
        var view = manager.GetView("MyView", (object)vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    [TestMethod]
    public void GetView_ByName_WithTypeAsObjectViewModel_TreatsAsViewModelType()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView")
            .WithViewModel(typeof(TestViewModel));
        services.AddSingleton<TestViewModel>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("MyView", (object)typeof(TestViewModel));
        Assert.IsNotNull(view);
        Assert.IsInstanceOfType<TestViewModel>(view.DataContext);
    }

    [TestMethod]
    public void GetView_ByName_WithObjectViewModel_UnregisteredName_ReturnsNull()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ViewManager>();
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var view = manager.GetView("NonExistent", (object)new TestViewModel());
        Assert.IsNull(view);
    }

    [TestMethod]
    public void GetView_GenericByName_WithObjectViewModel_ReturnsTypedView()
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TestView>(sp => new TestView())
            .WithName("MyView");
        var sp = services.BuildServiceProvider();
        var manager = sp.GetRequiredService<ViewManager>();

        var vm = new TestViewModel();
        var view = manager.GetView<TestView>("MyView", (object)vm);
        Assert.IsNotNull(view);
        Assert.AreSame(vm, view.DataContext);
    }

    // ===== ViewManagerResourceKey =====

    [TestMethod]
    public void ViewManagerResourceKey_IsExpectedValue()
    {
        Assert.AreEqual("Fachep.WpfUtils.ViewManager.ViewManagerResourceKey",
            ViewManager.ViewManagerResourceKey);
    }
}
