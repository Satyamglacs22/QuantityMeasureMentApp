using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementAppBusiness;
using QuantityMeasurementAppBusiness.Interfaces;
using QuantityMeasurementAppBusiness.Implementations;

namespace QuantityMeasurementApp.Tests.Model
{
    [TestClass]
    public class QuantityTests
    {
        [TestMethod]
        public void TestLengthEquality()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(1, LengthUnitConverter.Feet);
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(12, LengthUnitConverter.Inches);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestLengthAddition()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(1, LengthUnitConverter.Feet);
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(12, LengthUnitConverter.Inches);

            Quantity<IMeasurable> result = q1.Add(q2);

            Assert.AreEqual(2, result.Value, 0.01);
        }

        [TestMethod]
        public void TestVolumeConversion()
        {
            Quantity<IMeasurable> q = new Quantity<IMeasurable>(5, VolumeUnitConverter.Litre);

            Quantity<IMeasurable> result = q.ConvertTo(VolumeUnitConverter.Millilitre);

            Assert.AreEqual(5000, result.Value, 0.01);
        }

        [TestMethod]
        public void TestWeightDivision()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(10, WeightUnitConverter.Kilogram);
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(2, WeightUnitConverter.Kilogram);

            double result = q1.Divide(q2);

            Assert.AreEqual(5, result, 0.01);
        }
    }
}