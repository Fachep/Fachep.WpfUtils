using System.Windows;

using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils;

public class ViewManager(IServiceProvider serviceProvider)
{
    internal const string ViewManagerResourceKey = "Fachep.WpfUtils.ViewManager.ViewManagerResourceKey";

    public FrameworkElement? GetView(
        Type viewType,
        Type? viewModelType = null,
        Action<object>? viewModelCallback = null
    )
    {
        if (serviceProvider.GetService(viewType) is not FrameworkElement view)
        {
            return null;
        }

        viewModelType ??= (serviceProvider.GetService(typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(viewType)) as
            ViewBuilder.IViewWrapper)?.DefaultViewModelType;
        if (viewModelType is not null && serviceProvider.GetService(viewModelType) is { } viewModel)
        {
            viewModelCallback?.Invoke(viewModel);
            view.DataContext = viewModel;
        }

        return view;
    }

    public FrameworkElement? GetView(Type viewType, Action<object> viewModelCallback)
    {
        return GetView(viewType, null, viewModelCallback);
    }

    public TView? GetView<TView>(Type? viewModelType = null, Action<object>? viewModelCallback = null)
        where TView : FrameworkElement
    {
        return GetView(typeof(TView), viewModelType, viewModelCallback) as TView;
    }

    public TView? GetView<TView>(Action<object> viewModelCallback)
        where TView : FrameworkElement
    {
        return GetView<TView>(null, viewModelCallback);
    }

    public TView? GetView<TView, TViewModel>(Action<TViewModel>? viewModelCallback = null)
        where TView : FrameworkElement
    {
        Action<object>? callback = viewModelCallback is null ? null : vm => viewModelCallback((TViewModel)vm);
        return GetView(typeof(TView), typeof(TViewModel), callback) as TView;
    }

    public FrameworkElement? GetView(Type viewType, object viewModel)
    {
        if (viewModel is Type viewModelType)
        {
            // fix generic type resolve
            return GetView(viewType, viewModelType);
        }

        if (serviceProvider.GetService(viewType) is not FrameworkElement view)
        {
            return null;
        }

        view.DataContext = viewModel;
        return view;
    }

    public TView? GetView<TView>(object viewModel)
        where TView : FrameworkElement
    {
        return GetView(typeof(TView), viewModel) as TView;
    }

    public FrameworkElement? GetViewByViewModel(Type viewModelType, Action<object>? viewModelCallback = null)
    {
        var configType = typeof(ViewBuilder.IViewModelConfiguration<>).MakeGenericType(viewModelType);
        if (serviceProvider.GetService(configType) is ViewBuilder.IViewModelConfiguration<object> config)
        {
            return GetView(config.ViewType, viewModelType, viewModelCallback);
        }

        return null;
    }

    public FrameworkElement? GetViewByViewModel<TViewModel>(Action<TViewModel>? viewModelCallback = null)
    {
        Action<object>? callback = viewModelCallback is null ? null : vm => viewModelCallback((TViewModel)vm);
        return GetViewByViewModel(typeof(TViewModel), callback);
    }

    public TView? GetViewByViewModel<TViewModel, TView>(Action<TViewModel>? viewModelCallback = null)
        where TView : FrameworkElement
    {
        return GetViewByViewModel(viewModelCallback) as TView;
    }

    public FrameworkElement? GetViewByViewModel(Type viewModelType, object viewModel)
    {
        var configType = typeof(ViewBuilder.IViewModelConfiguration<>).MakeGenericType(viewModelType);
        if (serviceProvider.GetService(configType) is ViewBuilder.IViewModelConfiguration<object> config)
        {
            return GetView(config.ViewType, viewModel);
        }

        return null;
    }

    public FrameworkElement? GetViewByViewModel<TViewModel>(TViewModel viewModel)
    {
        if (viewModel is Type viewModelType)
        {
            // fix generic type resolve
            return GetViewByViewModel(viewModelType, null);
        }

        return GetViewByViewModel(typeof(TViewModel), viewModel);
    }

    public TView? GetViewByViewModel<TViewModel, TView>(TViewModel viewModel)
        where TView : FrameworkElement
    {
        return GetViewByViewModel(viewModel) as TView;
    }

    public FrameworkElement? GetView(
        string name,
        Type? viewModelType = null,
        bool keyedViewModel = false,
        Action<object>? viewModelCallback = null
    )
    {
        if (serviceProvider.GetKeyedService<ViewBuilder.IViewNameConfiguration>(name) is not { } config)
        {
            return null;
        }

        if (serviceProvider.GetService(config.ViewType) is not FrameworkElement view)
        {
            return null;
        }

        viewModelType ??=
            (serviceProvider.GetService(typeof(ViewBuilder.IViewWrapper<>).MakeGenericType(config.ViewType)) as
                ViewBuilder.IViewWrapper)?.DefaultViewModelType;
        if (viewModelType is null)
        {
            return view;
        }

        var viewModel = keyedViewModel
            ? serviceProvider.GetKeyedService(viewModelType, name)
            : serviceProvider.GetService(viewModelType);
        if (viewModel is not null)
        {
            viewModelCallback?.Invoke(viewModel);
            view.DataContext = viewModel;
        }

        return view;
    }

    public FrameworkElement? GetView(string name, Action<object> viewModelCallback, bool keyedViewModel = false)
    {
        return GetView(name, null, keyedViewModel, viewModelCallback);
    }

    public TView? GetView<TView>(
        string name,
        Type? viewModelType = null,
        bool keyedViewModel = false,
        Action<object>? viewModelCallback = null
    )
        where TView : FrameworkElement
    {
        return GetView(name, viewModelType, keyedViewModel, viewModelCallback) as TView;
    }

    public TView? GetView<TView>(string name, Action<object> viewModelCallback, bool keyedViewModel = false)
        where TView : FrameworkElement
    {
        return GetView<TView>(name, null, keyedViewModel, viewModelCallback);
    }

    public TView? GetView<TView, TViewModel>(
        string name,
        bool keyedViewModel = false,
        Action<TViewModel>? viewModelCallback = null
    )
        where TView : FrameworkElement
    {
        Action<object>? callback = viewModelCallback is null ? null : vm => viewModelCallback.Invoke((TViewModel)vm);
        return GetView(name, typeof(TViewModel), keyedViewModel, callback) as TView;
    }

    public FrameworkElement? GetView(string name, object viewModel)
    {
        if (viewModel is Type viewModelType)
        {
            // fix generic type resolve
            return GetView(name, viewModelType);
        }

        if (serviceProvider.GetKeyedService<ViewBuilder.IViewNameConfiguration>(name) is not { } config)
        {
            return null;
        }

        if (serviceProvider.GetService(config.ViewType) is not FrameworkElement view)
        {
            return null;
        }

        view.DataContext = viewModel;
        return view;
    }

    public TView? GetView<TView>(string name, object viewModel)
        where TView : FrameworkElement
    {
        return GetView(name, viewModel) as TView;
    }
}
