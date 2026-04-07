using System.Windows;
using System.Windows.Data;

namespace Fachep.WpfUtils.Tests;

[STATestClass]
public sealed class GetViewExtensionTests
{
    [TestMethod]
    public void Constructor_Default_SetsDefaultMode()
    {
        var ext = new GetViewExtension();
        Assert.AreEqual(GetViewMode.Auto, ext.Mode);
    }

    [TestMethod]
    public void Constructor_WithPath_SetsPath()
    {
        var ext = new GetViewExtension("SomeProperty");
        Assert.IsNotNull(ext.Path);
        Assert.AreEqual("SomeProperty", ext.Path.Path);
    }

    [TestMethod]
    public void Mode_CanBeSet()
    {
        var ext = new GetViewExtension { Mode = GetViewMode.ByName };
        Assert.AreEqual(GetViewMode.ByName, ext.Mode);
    }

    [TestMethod]
    public void ElementName_CanBeSetAndGet()
    {
        var ext = new GetViewExtension { ElementName = "myElement" };
        Assert.AreEqual("myElement", ext.ElementName);
    }

    [TestMethod]
    public void Path_CanBeSetAndGet()
    {
        var ext = new GetViewExtension { Path = new PropertyPath("Test") };
        Assert.AreEqual("Test", ext.Path.Path);
    }

    [TestMethod]
    public void Delay_CanBeSetAndGet()
    {
        var ext = new GetViewExtension { Delay = 500 };
        Assert.AreEqual(500, ext.Delay);
    }

    [TestMethod]
    public void FallbackValue_CanBeSetAndGet()
    {
        var fallback = new object();
        var ext = new GetViewExtension { FallbackValue = fallback };
        Assert.AreSame(fallback, ext.FallbackValue);
    }

    [TestMethod]
    public void TargetNullValue_CanBeSetAndGet()
    {
        var nullValue = new object();
        var ext = new GetViewExtension { TargetNullValue = nullValue };
        Assert.AreSame(nullValue, ext.TargetNullValue);
    }

    [TestMethod]
    public void Source_CanBeSetAndGet()
    {
        var source = new object();
        var ext = new GetViewExtension { Source = source };
        Assert.AreSame(source, ext.Source);
    }

    [TestMethod]
    public void RelativeSource_CanBeSetAndGet()
    {
        var rs = new RelativeSource(RelativeSourceMode.Self);
        var ext = new GetViewExtension { RelativeSource = rs };
        Assert.AreSame(rs, ext.RelativeSource);
    }

    [TestMethod]
    public void XPath_CanBeSetAndGet()
    {
        var ext = new GetViewExtension { XPath = "/root/child" };
        Assert.AreEqual("/root/child", ext.XPath);
    }
}
