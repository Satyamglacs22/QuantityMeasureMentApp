using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Model;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class QuantityWeightTests
    {
        private const double EPSILON = 0.0001;

        // Equality

        [TestMethod]
        public void TestEquality_KgToKg()
        {
            var w1 = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var w2 = new QuantityWeight(1.0, WeightUnit.Kilogram);

            Assert.IsTrue(w1.Equals(w2));
        }

        [TestMethod]
        public void TestEquality_KgToGram()
        {
            var w1 = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var w2 = new QuantityWeight(1000.0, WeightUnit.Gram);

            Assert.IsTrue(w1.Equals(w2));
        }

        // Conversion

        [TestMethod]
        public void TestConversion_KgToGram()
        {
            var w = new QuantityWeight(1.0, WeightUnit.Kilogram);

            var result = w.ConvertTo(WeightUnit.Gram);

            Assert.AreEqual(new QuantityWeight(1000.0, WeightUnit.Gram), result);
        }

        [TestMethod]
        public void TestConversion_KgToPound()
        {
            var w = new QuantityWeight(1.0, WeightUnit.Kilogram);

            var result = w.ConvertTo(WeightUnit.Pound);

            Assert.IsTrue(Math.Abs(result.Value - 2.20462) < EPSILON);
        }

        // Addition

        [TestMethod]
        public void TestAddition_KgPlusKg()
        {
            var w1 = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var w2 = new QuantityWeight(2.0, WeightUnit.Kilogram);

            var result = w1.Add(w2);

            Assert.AreEqual(new QuantityWeight(3.0, WeightUnit.Kilogram), result);
        }

        [TestMethod]
        public void TestAddition_KgPlusGram()
        {
            var w1 = new QuantityWeight(1.0, WeightUnit.Kilogram);
            var w2 = new QuantityWeight(1000.0, WeightUnit.Gram);

            var result = w1.Add(w2);

            Assert.AreEqual(new QuantityWeight(2.0, WeightUnit.Kilogram), result);
        }
    }
}