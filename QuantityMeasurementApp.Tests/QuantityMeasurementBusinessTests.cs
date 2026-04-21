using QuantityMeasurementAppBusiness;
using QuantityMeasurementAppBusiness.Implementations;
using QuantityMeasurementAppBusiness.Interfaces;
using QuantityMeasurementAppModels.Enums;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class QuantityMeasurementBusinessTests
    {
        [TestMethod]
        public void TestLength_OneFeetEqualsTwelveInch()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(1.0,  new LengthMeasurementImpl(LengthUnit.Feet));
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(12.0, new LengthMeasurementImpl(LengthUnit.Inch));
            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestLength_ConvertFeetToInch()
        {
            Quantity<IMeasurable> q      = new Quantity<IMeasurable>(1.0, new LengthMeasurementImpl(LengthUnit.Feet));
            Quantity<IMeasurable> result = q.ConvertTo(new LengthMeasurementImpl(LengthUnit.Inch));
            Assert.AreEqual(12.0, result.GetValue(), 0.01);
        }

        [TestMethod]
        public void TestLength_AddFeetAndInch()
        {
            Quantity<IMeasurable> q1     = new Quantity<IMeasurable>(1.0,  new LengthMeasurementImpl(LengthUnit.Feet));
            Quantity<IMeasurable> q2     = new Quantity<IMeasurable>(12.0, new LengthMeasurementImpl(LengthUnit.Inch));
            IMeasurable           target = new LengthMeasurementImpl(LengthUnit.Feet);
            Quantity<IMeasurable> result = q1.Add(q2, target);
            Assert.AreEqual(2.0, result.GetValue(), 0.01);
        }

        [TestMethod]
        public void TestLength_SubtractInchFromFeet()
        {
            Quantity<IMeasurable> q1     = new Quantity<IMeasurable>(2.0,  new LengthMeasurementImpl(LengthUnit.Feet));
            Quantity<IMeasurable> q2     = new Quantity<IMeasurable>(12.0, new LengthMeasurementImpl(LengthUnit.Inch));
            IMeasurable           target = new LengthMeasurementImpl(LengthUnit.Feet);
            Quantity<IMeasurable> result = q1.Subtract(q2, target);
            Assert.AreEqual(1.0, result.GetValue(), 0.01);
        }

        [TestMethod]
        public void TestLength_DivideTwoFeetByOneFoot()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(2.0, new LengthMeasurementImpl(LengthUnit.Feet));
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(1.0, new LengthMeasurementImpl(LengthUnit.Feet));
            double result = q1.Divide(q2);
            Assert.AreEqual(2.0, result, 0.01);
        }

        [TestMethod]
        public void TestWeight_OneKilogramEqualsThousandGram()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(1.0,    new WeightMeasurementImpl(WeightUnit.Kilogram));
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(1000.0, new WeightMeasurementImpl(WeightUnit.Gram));
            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestWeight_ConvertKilogramToGram()
        {
            Quantity<IMeasurable> q      = new Quantity<IMeasurable>(1.0, new WeightMeasurementImpl(WeightUnit.Kilogram));
            Quantity<IMeasurable> result = q.ConvertTo(new WeightMeasurementImpl(WeightUnit.Gram));
            Assert.AreEqual(1000.0, result.GetValue(), 0.01);
        }

        [TestMethod]
        public void TestWeight_AddKilogramAndGram()
        {
            Quantity<IMeasurable> q1     = new Quantity<IMeasurable>(1.0,   new WeightMeasurementImpl(WeightUnit.Kilogram));
            Quantity<IMeasurable> q2     = new Quantity<IMeasurable>(500.0, new WeightMeasurementImpl(WeightUnit.Gram));
            IMeasurable           target = new WeightMeasurementImpl(WeightUnit.Kilogram);
            Quantity<IMeasurable> result = q1.Add(q2, target);
            Assert.AreEqual(1.5, result.GetValue(), 0.01);
        }

        [TestMethod]
        public void TestVolume_OneLitreEqualsThousandMillilitre()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(1.0,    new VolumeMeasurementImpl(VolumeUnit.Litre));
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(1000.0, new VolumeMeasurementImpl(VolumeUnit.Millilitre));
            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestVolume_ConvertLitreToMillilitre()
        {
            Quantity<IMeasurable> q      = new Quantity<IMeasurable>(1.0, new VolumeMeasurementImpl(VolumeUnit.Litre));
            Quantity<IMeasurable> result = q.ConvertTo(new VolumeMeasurementImpl(VolumeUnit.Millilitre));
            Assert.AreEqual(1000.0, result.GetValue(), 0.01);
        }

        [TestMethod]
        public void TestVolume_AddLitreAndMillilitre()
        {
            Quantity<IMeasurable> q1     = new Quantity<IMeasurable>(1.0,   new VolumeMeasurementImpl(VolumeUnit.Litre));
            Quantity<IMeasurable> q2     = new Quantity<IMeasurable>(500.0, new VolumeMeasurementImpl(VolumeUnit.Millilitre));
            IMeasurable           target = new VolumeMeasurementImpl(VolumeUnit.Litre);
            Quantity<IMeasurable> result = q1.Add(q2, target);
            Assert.AreEqual(1.5, result.GetValue(), 0.01);
        }

        [TestMethod]
        public void TestTemperature_100CelsiusEquals212Fahrenheit()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(100.0, new TemperatureMeasurementImpl(TemperatureUnit.Celsius));
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(212.0, new TemperatureMeasurementImpl(TemperatureUnit.Fahrenheit));
            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestTemperature_ConvertCelsiusToFahrenheit()
        {
            Quantity<IMeasurable> q      = new Quantity<IMeasurable>(100.0, new TemperatureMeasurementImpl(TemperatureUnit.Celsius));
            Quantity<IMeasurable> result = q.ConvertTo(new TemperatureMeasurementImpl(TemperatureUnit.Fahrenheit));
            Assert.AreEqual(212.0, result.GetValue(), 0.01);
        }

        [TestMethod]
        public void TestTemperature_ConvertCelsiusToKelvin()
        {
            Quantity<IMeasurable> q      = new Quantity<IMeasurable>(100.0, new TemperatureMeasurementImpl(TemperatureUnit.Celsius));
            Quantity<IMeasurable> result = q.ConvertTo(new TemperatureMeasurementImpl(TemperatureUnit.Kelvin));
            Assert.AreEqual(373.15, result.GetValue(), 0.01);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestTemperature_AddThrowsNotSupportedException()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(100.0, new TemperatureMeasurementImpl(TemperatureUnit.Celsius));
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(50.0,  new TemperatureMeasurementImpl(TemperatureUnit.Celsius));
            q1.Add(q2);
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestDivide_ByZero_ThrowsDivideByZero()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(1.0, new LengthMeasurementImpl(LengthUnit.Feet));
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(0.0, new LengthMeasurementImpl(LengthUnit.Feet));
            q1.Divide(q2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAdd_MismatchedTypes_ThrowsArgumentException()
        {
            Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(1.0, new LengthMeasurementImpl(LengthUnit.Feet));
            Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(1.0, new WeightMeasurementImpl(WeightUnit.Kilogram));
            q1.Add(q2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructor_NullUnit_ThrowsArgumentException()
        {
            Quantity<IMeasurable> q = new Quantity<IMeasurable>(1.0, null!);
        }
    }
}