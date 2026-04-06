Fachep.WpfUtils.ViewManager
==============================
View management for WPF applications.

## Usages

```csharp
using Fachep.WpfUtils.ViewManager;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace YourNamespace;

public partial class App : Application
{
    public App() 
    {
        InitializeComponent();
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = this.GetView<MainWindow>(vm => 
        {
            /* modify view model here */
        });
        MainWindow = mainWindow;
        MainWindow.Visibility = Visibility.Visible;
        base.OnStartup(e);
    }
    
    [STAThread]
    public static void Main() 
    {
        var services = new ServiceCollection()
            .AddViews()
            .BuildServiceProvider();
        var app = services.GetRequiredService<App>();
        app.Run();
    }
}
```

```csharp
using Fachep.WpfUtils.ViewManager;
using System.Windows;

namespace YourNamespace;

[View]
[WithViewModel(typeof(MainWindowViewModel))]
public partial class MainWindow : Window // FrameworkElement
{
    public MainWindow()
    {
        InitializeComponent();
        Frame.Navigate(this.GetView("MainPage"));
    }
}
```

```csharp
using Fachep.WpfUtils.ViewManager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace YourNamespace;

public partial class MainPageViewModel(ViewManager viewManager) : ObservableObject
{
    [RelayCommand] 
    private void NavigateToAnotherPage() {
        var page = viewManager.GetViewByViewModel<AnotherPageViewModel>(vm => 
        {
            /* modify view model here */
        });
        CurrentPage = page;
    }
    
    [ObservableProperty]
    private object _currentPage;
}
```
