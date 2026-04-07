using CommunityToolkit.Mvvm.ComponentModel;

namespace Playground;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string CurrentPage { get; set; } = string.Empty;
}
