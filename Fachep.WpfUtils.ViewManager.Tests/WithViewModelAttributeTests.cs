using Microsoft.Extensions.DependencyInjection;

namespace Fachep.WpfUtils.Tests;

[TestClass]
public sealed class WithViewModelAttributeTests
{
    [TestMethod]
    public void Constructor_StoresViewModelType()
    {
        var attr = new WithViewModelAttribute(typeof(TestViewModel));
        Assert.AreEqual(typeof(TestViewModel), attr.ViewModelType);
    }

    [TestMethod]
    public void LifeTime_DefaultsToSingleton()
    {
        var attr = new WithViewModelAttribute(typeof(TestViewModel));
        Assert.AreEqual(ServiceLifetime.Singleton, attr.LifeTime);
    }

    [TestMethod]
    public void LifeTime_CanBeSet()
    {
        var attr = new WithViewModelAttribute(typeof(TestViewModel)) { LifeTime = ServiceLifetime.Transient };
        Assert.AreEqual(ServiceLifetime.Transient, attr.LifeTime);
    }

    [TestMethod]
    public void IsDefault_DefaultsToFalse()
    {
        var attr = new WithViewModelAttribute(typeof(TestViewModel));
        Assert.IsFalse(attr.IsDefault);
    }

    [TestMethod]
    public void IsDefault_CanBeSetToTrue()
    {
        var attr = new WithViewModelAttribute(typeof(TestViewModel)) { IsDefault = true };
        Assert.IsTrue(attr.IsDefault);
    }

    [TestMethod]
    public void AttributeUsage_ClassAndStruct_AllowMultiple_NotInherited()
    {
        var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(
            typeof(WithViewModelAttribute), typeof(AttributeUsageAttribute)
        )!;
        Assert.AreEqual(AttributeTargets.Class | AttributeTargets.Struct, usage.ValidOn);
        Assert.IsTrue(usage.AllowMultiple);
        Assert.IsFalse(usage.Inherited);
    }
}
