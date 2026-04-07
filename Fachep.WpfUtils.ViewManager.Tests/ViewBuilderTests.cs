using System.Windows;
using System.Windows.Controls;
using Fachep.WpfUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils.Tests;

[STATestClass]
public sealed class ViewBuilderTests
{
    [TestMethod]
    public void Create_Type_RegistersViewManagerAsSingleton()
    {
        var services = new ServiceCollection();
        ViewBuilder.Create(typeof(TestView), services);
        var sp = services.BuildServiceProvider();

        var vm = sp.GetService<ViewManager>();
        Assert.IsNotNull(vm);

        // Singleton: same instance
        var vm2 = sp.GetService<ViewManager>();
        Assert.AreSame(vm, vm2);
    }

    [TestMethod]
    public void Create_Type_ReturnsViewBuilder()
    {
        var services = new ServiceCollection();
        var builder = ViewBuilder.Create(typeof(TestView), services);
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void Create_WithInstance_RegistersViewResolvableFromDI()
    {
        var services = new ServiceCollection();
        var instance = new TestView();
        ViewBuilder.Create(typeof(TestView), services, instance);

        var sp = services.BuildServiceProvider();
        var resolved = sp.GetService(typeof(TestView));
        Assert.IsNotNull(resolved);
        Assert.AreSame(instance, resolved);
    }

    [TestMethod]
    public void Create_WithInstance_ReturnsSameInstanceOnMultipleResolves()
    {
        var services = new ServiceCollection();
        var instance = new TestView();
        ViewBuilder.Create(typeof(TestView), services, instance);

        var sp = services.BuildServiceProvider();
        var r1 = sp.GetService(typeof(TestView));
        var r2 = sp.GetService(typeof(TestView));
        Assert.AreSame(r1, r2);
    }

    [TestMethod]
    public void Create_WithImplType_Singleton_ReturnsSameInstanceOnMultipleResolves()
    {
        var services = new ServiceCollection();
        ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton, typeof(TestView));

        var sp = services.BuildServiceProvider();
        var r1 = sp.GetService(typeof(TestView)) as FrameworkElement;
        var r2 = sp.GetService(typeof(TestView)) as FrameworkElement;
        Assert.IsNotNull(r1);
        Assert.AreSame(r1, r2);
    }

    [TestMethod]
    public void Create_WithImplType_Transient_ReturnsDifferentInstancesOnMultipleResolves()
    {
        var services = new ServiceCollection();
        ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Transient, typeof(TestView));

        var sp = services.BuildServiceProvider();
        var r1 = sp.GetService(typeof(TestView)) as FrameworkElement;
        var r2 = sp.GetService(typeof(TestView)) as FrameworkElement;
        Assert.IsNotNull(r1);
        Assert.IsNotNull(r2);
        Assert.AreNotSame(r1, r2);
    }

    [TestMethod]
    public void Create_WithFactory_ResolvesUsingFactory()
    {
        var services = new ServiceCollection();
        var factoryView = new TestView();
        ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton,
            (Func<IServiceProvider, FrameworkElement>)(sp => factoryView));

        var sp = services.BuildServiceProvider();
        var resolved = sp.GetService(typeof(TestView));
        Assert.AreSame(factoryView, resolved);
    }

    [TestMethod]
    public void WithViewModel_RegistersViewModelConfiguration()
    {
        var services = new ServiceCollection();
        var builder = ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton, typeof(TestView));
        builder.WithViewModel(typeof(TestViewModel));

        var sp = services.BuildServiceProvider();
        var configType = typeof(ViewBuilder.IViewModelConfiguration<>).MakeGenericType(typeof(TestViewModel));
        var config = sp.GetService(configType) as ViewBuilder.IViewModelConfiguration<object>;
        Assert.IsNotNull(config);
        Assert.AreEqual(typeof(TestView), config.ViewType);
    }

    [TestMethod]
    public void WithViewModel_Default_SetsDefaultViewModelType()
    {
        var services = new ServiceCollection();
        var builder = ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton, typeof(TestView));
        builder.WithViewModel(typeof(TestViewModel), isDefault: true);

        var sp = services.BuildServiceProvider();
        var wrapperType = typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(typeof(TestView));
        var wrapper = sp.GetService(wrapperType) as ViewBuilder.IViewWrapper;
        Assert.IsNotNull(wrapper);
        Assert.AreEqual(typeof(TestViewModel), wrapper.DefaultViewModelType);
    }

    [TestMethod]
    public void WithViewModel_NotDefault_SetsLastViewModelType()
    {
        var services = new ServiceCollection();
        var builder = ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton, typeof(TestView));
        builder.WithViewModel(typeof(TestViewModel));

        var sp = services.BuildServiceProvider();
        var wrapperType = typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(typeof(TestView));
        var wrapper = sp.GetService(wrapperType) as ViewBuilder.IViewWrapper;
        Assert.IsNotNull(wrapper);
        Assert.AreEqual(typeof(TestViewModel), wrapper.DefaultViewModelType);
    }

    [TestMethod]
    public void WithViewModel_DefaultTakesPrecedenceOverLast()
    {
        var services = new ServiceCollection();
        var builder = ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton, typeof(TestView));
        builder.WithViewModel(typeof(TestViewModel));
        builder.WithViewModel(typeof(TestViewModel2), isDefault: true);

        var sp = services.BuildServiceProvider();
        var wrapperType = typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(typeof(TestView));
        var wrapper = sp.GetService(wrapperType) as ViewBuilder.IViewWrapper;
        Assert.IsNotNull(wrapper);
        Assert.AreEqual(typeof(TestViewModel2), wrapper.DefaultViewModelType);
    }

    [TestMethod]
    public void WithName_RegistersKeyedNameConfiguration()
    {
        var services = new ServiceCollection();
        var builder = ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton, typeof(TestView));
        builder.WithName("TestName");

        var sp = services.BuildServiceProvider();
        var config = sp.GetKeyedService<ViewBuilder.IViewNameConfiguration>("TestName");
        Assert.IsNotNull(config);
        Assert.AreEqual(typeof(TestView), config.ViewType);
    }

    [TestMethod]
    public void WithName_MultipleNames_AllRegistered()
    {
        var services = new ServiceCollection();
        var builder = ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton, typeof(TestView));
        builder.WithName("Name1").WithName("Name2");

        var sp = services.BuildServiceProvider();
        var config1 = sp.GetKeyedService<ViewBuilder.IViewNameConfiguration>("Name1");
        var config2 = sp.GetKeyedService<ViewBuilder.IViewNameConfiguration>("Name2");
        Assert.IsNotNull(config1);
        Assert.IsNotNull(config2);
        Assert.AreEqual(typeof(TestView), config1.ViewType);
        Assert.AreEqual(typeof(TestView), config2.ViewType);
    }

    [TestMethod]
    public void WithViewModel_WithoutWrapper_DoesNotThrow()
    {
        // ViewBuilder.Create(type, services) doesn't set _viewWrapper
        var services = new ServiceCollection();
        var builder = ViewBuilder.Create(typeof(TestView), services);
        builder.WithViewModel(typeof(TestViewModel));

        // Should still register the config without throwing
        var sp = services.BuildServiceProvider();
        var configType = typeof(ViewBuilder.IViewModelConfiguration<>).MakeGenericType(typeof(TestViewModel));
        var config = sp.GetService(configType) as ViewBuilder.IViewModelConfiguration<object>;
        Assert.IsNotNull(config);
    }

    [TestMethod]
    public void WithViewModel_ReturnsSameBuilder_ForChaining()
    {
        var services = new ServiceCollection();
        var builder = ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton, typeof(TestView));
        var result = builder.WithViewModel(typeof(TestViewModel));
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void WithName_ReturnsSameBuilder_ForChaining()
    {
        var services = new ServiceCollection();
        var builder = ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton, typeof(TestView));
        var result = builder.WithName("Test");
        Assert.AreSame(builder, result);
    }

    [TestMethod]
    public void Create_WithInstance_WrapperCloneReturnsNewInstance()
    {
        var services = new ServiceCollection();
        var instance = new TestView();
        ViewBuilder.Create(typeof(TestView), services, instance);

        var sp = services.BuildServiceProvider();
        var wrapperType = typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(typeof(TestView));
        var wrapper = sp.GetService(wrapperType) as ViewBuilder.IViewWrapper;
        Assert.IsNotNull(wrapper);

        var cloned = wrapper.Clone();
        Assert.IsNotNull(cloned);
        Assert.AreNotSame(wrapper, cloned);
    }

    [TestMethod]
    public void InstanceWrapper_GetInstance_ReturnsSameInstance()
    {
        var services = new ServiceCollection();
        var instance = new TestView();
        ViewBuilder.Create(typeof(TestView), services, instance);

        var sp = services.BuildServiceProvider();
        var wrapperType = typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(typeof(TestView));
        var wrapper = sp.GetService(wrapperType) as ViewBuilder.IViewWrapper;
        Assert.IsNotNull(wrapper);

        var result = wrapper.GetInstance(sp);
        Assert.AreSame(instance, result);
    }

    [TestMethod]
    public void InstanceWrapper_DefaultViewModelType_NullByDefault()
    {
        var services = new ServiceCollection();
        var instance = new TestView();
        ViewBuilder.Create(typeof(TestView), services, instance);

        var sp = services.BuildServiceProvider();
        var wrapperType = typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(typeof(TestView));
        var wrapper = sp.GetService(wrapperType) as ViewBuilder.IViewWrapper;
        Assert.IsNotNull(wrapper);
        Assert.IsNull(wrapper.DefaultViewModelType);
    }

    [TestMethod]
    public void InstanceWrapper_WithViewModelType_SetsDefault()
    {
        var services = new ServiceCollection();
        var instance = new TestView();
        var builder = ViewBuilder.Create(typeof(TestView), services, instance);
        builder.WithViewModel(typeof(TestViewModel), isDefault: true);

        var sp = services.BuildServiceProvider();
        var wrapperType = typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(typeof(TestView));
        var wrapper = sp.GetService(wrapperType) as ViewBuilder.IViewWrapper;
        Assert.IsNotNull(wrapper);
        Assert.AreEqual(typeof(TestViewModel), wrapper.DefaultViewModelType);
    }

    [TestMethod]
    public void FactoryWrapper_Clone_CreatesNewWrapper()
    {
        var services = new ServiceCollection();
        var view = new TestView();
        ViewBuilder.Create(typeof(TestView), services, ServiceLifetime.Singleton,
            (Func<IServiceProvider, FrameworkElement>)(sp => view));

        var sp = services.BuildServiceProvider();
        var wrapperType = typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(typeof(TestView));
        var wrapper = sp.GetService(wrapperType) as ViewBuilder.IViewWrapper;
        Assert.IsNotNull(wrapper);

        var cloned = wrapper.Clone();
        Assert.IsNotNull(cloned);
        Assert.AreNotSame(wrapper, cloned);
    }
}
