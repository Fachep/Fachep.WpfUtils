using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils.Tests;

[STATestClass]
public class ViewManagerTests
{
    [TestMethod]
    public void GetView_WithValidViewTypeAndNoViewModel_ReturnsView()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestView>();
        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);

        var result = viewManager.GetView(typeof(TestView));

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(TestView));
        Assert.IsNull(result.DataContext);
    }

    [TestMethod]
    public void GetView_WithValidViewTypeAndViewModelType_ReturnsViewWithDataContextSet()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestView>();
        services.AddTransient<TestViewModel>();
        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);

        var result = viewManager.GetView(typeof(TestView), typeof(TestViewModel));

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(TestView));
        Assert.IsNotNull(result.DataContext);
        Assert.IsInstanceOfType(result.DataContext, typeof(TestViewModel));
    }

    [TestMethod]
    public void GetView_WithUnregisteredViewType_ReturnsNull()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);

        var result = viewManager.GetView(typeof(TestView));

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetView_WithViewModelCallback_InvokesCallbackAndSetsDataContext()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestView>();
        services.AddTransient<TestViewModel>();
        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);
        var callbackInvoked = false;

        var result = viewManager.GetView(typeof(TestView), typeof(TestViewModel), vm => callbackInvoked = true);

        Assert.IsNotNull(result);
        Assert.IsTrue(callbackInvoked);
        Assert.IsNotNull(result.DataContext);
    }

    [TestMethod]
    public void GetView_GenericTyped_WithValidTypes_ReturnsStronglyTypedViewAndSetsDataContext()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestView>();
        services.AddTransient<TestViewModel>();
        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);

        var result = viewManager.GetView<TestView, TestViewModel>();

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.DataContext, typeof(TestViewModel));
    }

    [TestMethod]
    public void GetViewByName_WithValidName_ReturnsViewWithDataContext()
    {
        var viewInstance = new TestView();
        var services = new ServiceCollection();
        services.AddTransient<TestViewModel>();

        // Use extension method to configure
        services.AddView(viewInstance).WithName("MyView");

        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);

        var result = viewManager.GetView("MyView", typeof(TestViewModel));

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(TestView));
        Assert.IsInstanceOfType(result.DataContext, typeof(TestViewModel));
    }

    [TestMethod]
    public void GetViewByName_WithUnregisteredName_ReturnsNull()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);

        var result = viewManager.GetView("UnregisteredView", typeof(TestViewModel));

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetViewByViewModel_WithRegisteredConfiguration_ReturnsViewAndSetsDataContext()
    {
        var viewInstance = new TestView();
        var services = new ServiceCollection();
        services.AddTransient<TestViewModel>();

        // Use extension method to configure
        services.AddView(viewInstance).WithViewModel(typeof(TestViewModel));

        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);

        var result = viewManager.GetViewByViewModel(typeof(TestViewModel));

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(TestView));
        Assert.IsInstanceOfType(result.DataContext, typeof(TestViewModel));
    }

    [TestMethod]
    public void GetViewByViewModel_Generic_InvokesCallbackAndSetsDataContext()
    {
        var viewInstance = new TestView();
        var services = new ServiceCollection();
        services.AddTransient<TestViewModel>();
        services.AddView(viewInstance).WithViewModel(typeof(TestViewModel));
        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);
        var callbackInvoked = false;

        var result = viewManager.GetViewByViewModel<TestViewModel>(vm => callbackInvoked = true);

        Assert.IsNotNull(result);
        Assert.IsTrue(callbackInvoked);
        Assert.IsInstanceOfType(result.DataContext, typeof(TestViewModel));
    }

    [TestMethod]
    public void GetViewByViewModel_GenericTyped_ReturnsStronglyTypedView()
    {
        var viewInstance = new TestView();
        var services = new ServiceCollection();
        services.AddTransient<TestViewModel>();
        services.AddView(viewInstance).WithViewModel(typeof(TestViewModel));
        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);

        var result = viewManager.GetViewByViewModel<TestViewModel, TestView>();

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(TestView));
    }

    [TestMethod]
    public void GetViewByName_WithKeyedViewModel_ReturnsViewWithKeyedDataContext()
    {
        var viewInstance = new TestView();
        var services = new ServiceCollection();
        services.AddKeyedTransient<TestViewModel>("MyView");
        services.AddView(viewInstance).WithName("MyView");

        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);

        var result = viewManager.GetView("MyView", typeof(TestViewModel), true);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.DataContext, typeof(TestViewModel));
    }

    [TestMethod]
    public void GetViewByName_Generic_InvokesCallbackAndSetsDataContext()
    {
        var viewInstance = new TestView();
        var services = new ServiceCollection();
        services.AddTransient<TestViewModel>();
        services.AddView(viewInstance).WithName("MyView");

        var provider = services.BuildServiceProvider();
        var viewManager = new ViewManager(provider);
        var callbackInvoked = false;

        var result = viewManager.GetView<TestView, TestViewModel>("MyView", false, vm => callbackInvoked = true);

        Assert.IsNotNull(result);
        Assert.IsTrue(callbackInvoked);
        Assert.IsInstanceOfType(result, typeof(TestView));
        Assert.IsInstanceOfType(result.DataContext, typeof(TestViewModel));
    }

    private class TestView : FrameworkElement
    {
    }

    private class TestViewModel
    {
    }
}