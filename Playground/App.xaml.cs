using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Playground;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App() 
    {
        InitializeComponent();
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = this.GetView<MainWindow>();
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