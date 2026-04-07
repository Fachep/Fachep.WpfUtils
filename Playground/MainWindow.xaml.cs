using System.Windows;

using Fachep.WpfUtils;

namespace Playground;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
[WithViewModel(typeof(MainWindowViewModel))]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
