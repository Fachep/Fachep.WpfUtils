using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils;

[MarkupExtensionReturnType(typeof(object))]
public class GetViewExtension : MarkupExtension
{
    private readonly Binding _binding;
    private ViewManager? _viewManager;

    public GetViewExtension()
    {
        _binding = new Binding
        {
            Mode = BindingMode.OneWay, Converter = new GetViewConverter(), ConverterParameter = this,
        };
    }

    public GetViewExtension(string path) : this()
    {
        _binding.Path = new PropertyPath(path);
    }

    public string ElementName
    {
        get => _binding.ElementName;
        set => _binding.ElementName = value;
    }

    public PropertyPath Path
    {
        get => _binding.Path;
        set => _binding.Path = value;
    }

    public RelativeSource RelativeSource
    {
        get => _binding.RelativeSource;
        set => _binding.RelativeSource = value;
    }

    public object Source
    {
        get => _binding.Source;
        set => _binding.Source = value;
    }

    public string XPath
    {
        get => _binding.XPath;
        set => _binding.XPath = value;
    }

    public int Delay
    {
        get => _binding.Delay;
        set => _binding.Delay = value;
    }

    public object FallbackValue
    {
        get => _binding.FallbackValue;
        set => _binding.FallbackValue = value;
    }

    public object TargetNullValue
    {
        get => _binding.TargetNullValue;
        set => _binding.TargetNullValue = value;
    }

    [DefaultValue(GetViewMode.Auto)]
    public GetViewMode Mode { get; set; }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        var target = serviceProvider.GetService<IProvideValueTarget>();
        switch (target?.TargetObject)
        {
            case Setter:
                return _binding;
            case DependencyObject targetObject:
                _viewManager ??= targetObject.ViewManager;
                break;
            default:
                throw new NotSupportedException(
                    $"Target object of type {target?.TargetObject.GetType()} is not supported."
                );
        }

        return _binding.ProvideValue(serviceProvider);
    }

    private class GetViewConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(value);
#else
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
#endif
            var ext = (GetViewExtension)parameter!;
            var mode = ext.Mode;

            var view = mode switch
            {
                GetViewMode.Auto => value switch
                {
                    string name => ext._viewManager!.GetView(name),
                    Type viewType => ext._viewManager!.GetView(viewType),
                    _ => ext._viewManager!.GetViewByViewModel(value.GetType(), value),
                },
                GetViewMode.ByName => ext._viewManager!.GetView((string)value),
                GetViewMode.ByViewModelType => ext._viewManager!.GetViewByViewModel((Type)value, null),
                GetViewMode.ByViewType => ext._viewManager!.GetView((Type)value),
                GetViewMode.ByTypeOfViewModelInstance => ext._viewManager!.GetViewByViewModel(value.GetType(), value),
                _ => throw new ArgumentOutOfRangeException(nameof(Mode)),
            };
            return view;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
