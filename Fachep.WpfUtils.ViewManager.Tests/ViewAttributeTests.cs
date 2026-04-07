using Fachep.WpfUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils.Tests;

[TestClass]
public sealed class ViewAttributeTests
{
    [TestMethod]
    public void Constructor_WithNoNames_HasEmptyNamesArray()
    {
        var attr = new ViewAttribute();
        Assert.IsNotNull(attr.Names);
        Assert.AreEqual(0, attr.Names.Length);
    }

    [TestMethod]
    public void Constructor_WithNames_StoresNames()
    {
        var attr = new ViewAttribute("View1", "View2");
        CollectionAssert.AreEqual(new[] { "View1", "View2" }, attr.Names);
    }

    [TestMethod]
    public void Constructor_WithSingleName_StoresName()
    {
        var attr = new ViewAttribute("MyView");
        Assert.AreEqual(1, attr.Names.Length);
        Assert.AreEqual("MyView", attr.Names[0]);
    }

    [TestMethod]
    public void LifeTime_DefaultsToSingleton()
    {
        var attr = new ViewAttribute();
        Assert.AreEqual(ServiceLifetime.Singleton, attr.LifeTime);
    }

    [TestMethod]
    public void LifeTime_CanBeSet()
    {
        var attr = new ViewAttribute { LifeTime = ServiceLifetime.Transient };
        Assert.AreEqual(ServiceLifetime.Transient, attr.LifeTime);
    }

    [TestMethod]
    public void ViewType_DefaultsToNull()
    {
        var attr = new ViewAttribute();
        Assert.IsNull(attr.ViewType);
    }

    [TestMethod]
    public void ViewType_CanBeSet()
    {
        var attr = new ViewAttribute { ViewType = typeof(TestView) };
        Assert.AreEqual(typeof(TestView), attr.ViewType);
    }

    [TestMethod]
    public void AttributeUsage_ClassOnly_NotInherited()
    {
        var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(
            typeof(ViewAttribute), typeof(AttributeUsageAttribute))!;
        Assert.AreEqual(AttributeTargets.Class, usage.ValidOn);
        Assert.IsFalse(usage.Inherited);
    }
}
