using System.Windows.Controls;
using Fachep.WpfUtils;

namespace Playground;

[View]
[WithViewModel(typeof(Page1ViewModel))]
public partial class Page1 : Page
{
    public Page1()
    {
        InitializeComponent();
    }
}