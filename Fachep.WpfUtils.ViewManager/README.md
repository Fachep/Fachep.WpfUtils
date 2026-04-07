Fachep.WpfUtils.ViewManager
==============================
![GitHub License](https://img.shields.io/github/license/Fachep/Fachep.WpfUtils)
[![Build ViewManager](https://github.com/Fachep/Fachep.WpfUtils/actions/workflows/build-ViewManager.yml/badge.svg?event=push)](https://github.com/Fachep/Fachep.WpfUtils/actions/workflows/build-ViewManager.yml)
[![GitHub Release](https://img.shields.io/github/v/release/Fachep/Fachep.WpfUtils?filter=ViewManager-v*)][ViewManager-Release]
[![NuGet Version](https://img.shields.io/nuget/v/Fachep.WpfUtils.ViewManager)][ViewManager-NuGet]
[![NuGet Downloads](https://img.shields.io/nuget/dt/Fachep.WpfUtils.ViewManager)][ViewManager-NuGet]

[Release][ViewManager-Release] | [NuGet][ViewManager-NuGet] | [Usages](#usages)

View management for WPF applications.

Supported frameworks: .NET FX 4.6.2 or later, .NET 8.0 or later.

## Installation

```bash
dotnet add package Fachep.WpfUtils.ViewManager
```

## Features

### View Registration

#### By Convention

- MainWindow <-> MainViewModel
- HomePage <-> HomeViewModel
- MyView <-> MyViewModel

#### By Code

```csharp
[View]
[WithViewModel(typeof(MyViewModel), IsDefault = true)]
public partial class MyView : UserControl
```
```csharp
services.AddView<AboutPage>("About");
```

### View Resolution

```csharp
var view = viewManager.GetViewByViewModel(new MyViewModel());
```
```csharp
var view = viewManager.GetView("About");
```
```csharp
var view = this.GetView<AboutPage>();
```
```xaml
<Frame Content="{wpfUtils:GetView CurrentPage}" NavigationUIVisibility="Hidden" />
```

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

```xaml
<Window x:Class="YourNamespace.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:wpfUtils="clr-namespace:Fachep.WpfUtils;assembly=Fachep.WpfUtils.ViewManager"
        Title="MainWindow" Height="450" Width="800">

    <StackPanel>
        <ComboBox SelectedValue="{Binding CurrentPage}">
            <ComboBoxItem Content="Page1" />
            <ComboBoxItem Content="Page2" />
        </ComboBox>
        <Button Content="Go" Command="{Binding GoCommand}" />
        <Frame Content="{wpfUtils:GetView CurrentPage}" Height="350" NavigationUIVisibility="Hidden" />
    </StackPanel>
</Window>
```

```csharp
using Fachep.WpfUtils.ViewManager;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace YourNamespace;

public partial class MainViewModel(ViewManager viewManager) : ObservableObject
{
    [RelayCommand]
    private void Go() {
        CurrentPage = new AnotherViewModel();
    }

    [ObservableProperty]
    private object _currentPage;
}
```

[ViewManager-Release]: https://github.com/Fachep/Fachep.WpfUtils/releases?q=ViewManager&expanded=true
[ViewManager-NuGet]: https://www.nuget.org/packages/Fachep.WpfUtils.ViewManager/
