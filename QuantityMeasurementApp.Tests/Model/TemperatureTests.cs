using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementAppBusiness;
using QuantityMeasurementAppBusiness.Interfaces;
using QuantityMeasurementAppBusiness.Implementations;
using System;

namespace QuantityMeasurementApp.Tests.Model
{
    [TestClass]
    public class TemperatureTests
    {
        [TestMethod]
        public void TestTemperatureEquality()
        {
            Quantity<IMeasurable> t1 = new Quantity<IMeasurable>(0, TemperatureUnitConverter.CELSIUS);
            Quantity<IMeasurable> t2 = new Quantity<IMeasurable>(32, TemperatureUnitConverter.FAHRENHEIT);

            Assert.IsTrue(t1.Equals(t2));
        }

        [TestMethod]
        public void TestTemperatureConversion()
        {
            Quantity<IMeasurable> t = new Quantity<IMeasurable>(100, TemperatureUnitConverter.CELSIUS);

            Quantity<IMeasurable> result = t.ConvertTo(TemperatureUnitConverter.FAHRENHEIT);

            Assert.AreEqual(212, result.Value, 0.01);
        }

        [TestMethod]
        public void TestKelvinConversion()
        {
            Quantity<IMeasurable> t = new Quantity<IMeasurable>(0, TemperatureUnitConverter.CELSIUS);

            Quantity<IMeasurable> result = t.ConvertTo(TemperatureUnitConverter.KELVIN);

            Assert.AreEqual(273.15, result.Value, 0.01);
        }
[TestMethod]
public void TestTemperatureAddition_NotAllowed()
{
    Quantity<IMeasurable> t1 = new Quantity<IMeasurable>(100, TemperatureUnitConverter.CELSIUS);
    Quantity<IMeasurable> t2 = new Quantity<IMeasurable>(50, TemperatureUnitConverter.CELSIUS);

    try
    {
        t1.Add(t2);
        Assert.Fail("Expected NotSupportedException was not thrown");
    }
    catch (NotSupportedException)
    {
        // Expected - test passes
    }
}
    }
}