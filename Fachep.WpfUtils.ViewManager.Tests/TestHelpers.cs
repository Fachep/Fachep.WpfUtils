using System.Windows;
using System.Windows.Controls;

using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils.Tests;

// --- Simple test views ---

public class TestView : UserControl;

public class TestView2 : UserControl;

public class TestWindow : Window;

public class TestPage : Page;

// --- Views with ViewAttribute ---

[View("MyView")]
public class AttributedView : UserControl;

[View("NamedView1", "NamedView2")]
public class MultiNamedView : UserControl;

[View(LifeTime = ServiceLifetime.Transient)]
public class TransientAttributedView : UserControl;

[View(ViewType = typeof(TestViewBase))]
public class CustomViewTypeView : TestViewBase;

public class TestViewBase : UserControl;

// --- Views with WithViewModelAttribute ---

[View("VmView")]
[WithViewModel(typeof(TestViewModel))]
public class ViewWithViewModel : UserControl;

[View("MultiVmView")]
[WithViewModel(typeof(TestViewModel))]
[WithViewModel(typeof(TestViewModel2), IsDefault = true)]
public class ViewWithMultipleViewModels : UserControl;

[View("TransientVmView")]
[WithViewModel(typeof(TestViewModel), LifeTime = ServiceLifetime.Transient)]
public class ViewWithTransientViewModel : UserControl;

// --- ViewModels ---

public class TestViewModel;

public class TestViewModel2;

public class TestViewModel3;

// --- Convention-based discovery types ---
// These follow naming convention: XxxView + XxxViewModel

[View]
public class OrderView : UserControl;

public class OrderViewModel;

// Page/Window with conventional names (auto-discovered without [View])
public class SettingsPage : Page;

public class SettingsViewModel;

public class MainWindow2 : Window;

// --- Helper to build ServiceProvider for tests ---

internal static class TestServiceProviderFactory
{
    public static IServiceProvider BuildWithView<TView>(
        Action<ViewBuilder>? configure = null,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
        where TView : FrameworkElement
    {
        var services = new ServiceCollection();
        var builder = services.AddSingletonView<TView>();
        configure?.Invoke(builder);
        return services.BuildServiceProvider();
    }

    public static IServiceProvider BuildWithViewAndViewModel<TView, TViewModel>(
        ServiceLifetime viewLifetime = ServiceLifetime.Singleton,
        ServiceLifetime vmLifetime = ServiceLifetime.Singleton
    )
        where TView : FrameworkElement
        where TViewModel : class
    {
        var services = new ServiceCollection();
        services.AddSingletonView<TView>()
            .WithViewModel(typeof(TViewModel));
        ((IServiceCollection)services).Add(new ServiceDescriptor(typeof(TViewModel), typeof(TViewModel), vmLifetime));
        return services.BuildServiceProvider();
    }
}
