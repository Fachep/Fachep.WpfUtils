using Fachep.WpfUtils;

namespace System.Windows;

public static class FrameworkElementExtensions
{
    extension(FrameworkElement element)
    {
        public ViewManager ViewManager => (ViewManager)element.FindResource(ViewManager.ViewManagerResourceKey);

        public FrameworkElement? GetView(Type viewType, Type? viewModelType = null,
            Action<object>? viewModelCallback = null)
        {
            return element.ViewManager.GetView(viewType, viewModelType, viewModelCallback);
        }

        public TView? GetView<TView>(Type? viewModelType = null, Action<object>? viewModelCallback = null)
            where TView : FrameworkElement
        {
            return element.ViewManager.GetView<TView>(viewModelType, viewModelCallback);
        }

        public TView? GetView<TView, TViewModel>(Action<TViewModel>? viewModelCallback = null)
            where TView : FrameworkElement
        {
            return element.ViewManager.GetView<TView, TViewModel>(viewModelCallback);
        }

        public FrameworkElement? GetViewByViewModel(Type viewModelType, Action<object>? viewModelCallback = null)
        {
            return element.ViewManager.GetViewByViewModel(viewModelType, viewModelCallback);
        }

        public FrameworkElement? GetViewByViewModel<TViewModel>(Action<TViewModel>? viewModelCallback = null)
        {
            return element.ViewManager.GetViewByViewModel(viewModelCallback);
        }

        public TView? GetViewByViewModel<TViewModel, TView>(Action<TViewModel>? viewModelCallback = null)
            where TView : FrameworkElement
        {
            return element.ViewManager.GetViewByViewModel<TViewModel, TView>(viewModelCallback);
        }

        public FrameworkElement? GetView(string name, Type? viewModelType = null, bool keyedViewModel = false,
            Action<object>? viewModelCallback = null)
        {
            return element.ViewManager.GetView(name, viewModelType, keyedViewModel, viewModelCallback);
        }

        public TView? GetView<TView>(string name, Type? viewModelType = null, bool keyedViewModel = false,
            Action<object>? viewModelCallback = null)
            where TView : FrameworkElement
        {
            return element.ViewManager.GetView<TView>(name, viewModelType, keyedViewModel, viewModelCallback);
        }

        public TView? GetView<TView, TViewModel>(string name, bool keyedViewModel = false,
            Action<TViewModel>? viewModelCallback = null)
            where TView : FrameworkElement
        {
            return element.ViewManager.GetView<TView, TViewModel>(name, keyedViewModel, viewModelCallback);
        }
    }
}