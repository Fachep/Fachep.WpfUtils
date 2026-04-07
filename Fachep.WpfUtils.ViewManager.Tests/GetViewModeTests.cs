using Fachep.WpfUtils;

namespace Fachep.WpfUtils.Tests;

[TestClass]
public sealed class GetViewModeTests
{
    [TestMethod]
    public void Enum_HasExpectedValues()
    {
        Assert.AreEqual(0, (int)GetViewMode.Auto);
        Assert.AreEqual(1, (int)GetViewMode.ByName);
        Assert.AreEqual(2, (int)GetViewMode.ByViewModelType);
        Assert.AreEqual(3, (int)GetViewMode.ByViewType);
        Assert.AreEqual(4, (int)GetViewMode.ByTypeOfViewModelInstance);
    }

    [TestMethod]
    public void Enum_HasFiveValues()
    {
        var values = Enum.GetValues(typeof(GetViewMode));
        Assert.AreEqual(5, values.Length);
    }
}
