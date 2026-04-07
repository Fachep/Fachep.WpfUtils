namespace Fachep.WpfUtils;

public enum GetViewMode
{
    Auto, // string => ByName, Type => ByViewType, object => ByTypeOfViewModelInstance
    ByName,
    ByViewModelType,
    ByViewType,
    ByTypeOfViewModelInstance,
}
