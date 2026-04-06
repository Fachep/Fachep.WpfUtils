using Fachep.WpfUtils;

namespace System.Windows;

public static class ApplicationExtensions
{
    extension(Application app)
    {
        public ViewManager ViewManager => (ViewManager)app.Resources[ViewManager.ViewManagerResourceKey]!;

        public FrameworkElement? GetView(Type viewType, Type? viewModelType = null,
            Action<object>? viewModelCallback = null)
        {
            return app.ViewManager.GetView(viewType, viewModelType, viewModelCallback);
        }

        public TView? GetView<TView>(Type? viewModelType = null, Action<object>? viewModelCallback = null)
            where TView : FrameworkElement
        {
            return app.ViewManager.GetView<TView>(viewModelType, viewModelCallback);
        }

        public TView? GetView<TView, TViewModel>(Action<TViewModel>? viewModelCallback = null)
            where TView : FrameworkElement
        {
            return app.ViewManager.GetView<TView, TViewModel>(viewModelCallback);
        }

        public FrameworkElement? GetViewByViewModel(Type viewModelType, Action<object>? viewModelCallback = null)
        {
            return app.ViewManager.GetViewByViewModel(viewModelType, viewModelCallback);
        }

        public FrameworkElement? GetViewByViewModel<TViewModel>(Action<TViewModel>? viewModelCallback = null)
        {
            return app.ViewManager.GetViewByViewModel(viewModelCallback);
        }

        public TView? GetViewByViewModel<TViewModel, TView>(Action<TViewModel>? viewModelCallback = null)
            where TView : FrameworkElement
        {
            return app.ViewManager.GetViewByViewModel<TViewModel, TView>(viewModelCallback);
        }

        public FrameworkElement? GetView(string name, Type? viewModelType = null, bool keyedViewModel = false,
            Action<object>? viewModelCallback = null)
        {
            return app.ViewManager.GetView(name, viewModelType, keyedViewModel, viewModelCallback);
        }

        public TView? GetView<TView>(string name, Type? viewModelType = null, bool keyedViewModel = false,
            Action<object>? viewModelCallback = null)
            where TView : FrameworkElement
        {
            return app.ViewManager.GetView<TView>(name, viewModelType, keyedViewModel, viewModelCallback);
        }

        public TView? GetView<TView, TViewModel>(string name, bool keyedViewModel = false,
            Action<TViewModel>? viewModelCallback = null)
            where TView : FrameworkElement
        {
            return app.ViewManager.GetView<TView, TViewModel>(name, keyedViewModel, viewModelCallback);
        }
    }
}