using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class QuantityLengthTests
{
    private const double EPSILON = 0.000001;

    [TestMethod]
    public void TestConversion_FeetToInches()
    {
        double result = QuantityLength.Convert(1.0, LengthUnit.Feet, LengthUnit.Inches);
        Assert.IsTrue(Math.Abs(result - 12.0) < EPSILON);
    }

    [TestMethod]
    public void TestConversion_InchesToFeet()
    {
        double result = QuantityLength.Convert(24.0, LengthUnit.Inches, LengthUnit.Feet);
        Assert.IsTrue(Math.Abs(result - 2.0) < EPSILON);
    }

    [TestMethod]
    public void TestConversion_YardsToInches()
    {
        double result = QuantityLength.Convert(1.0, LengthUnit.Yards, LengthUnit.Inches);
        Assert.IsTrue(Math.Abs(result - 36.0) < EPSILON);
    }

    [TestMethod]
    public void TestConversion_CentimetersToInches()
    {
        double result = QuantityLength.Convert(2.54, LengthUnit.Centimeters, LengthUnit.Inches);
        Assert.IsTrue(Math.Abs(result - 1.0) < EPSILON);
    }

    [TestMethod]
    public void TestConversion_RoundTrip()
    {
        double original = 5.0;

        double toYards = QuantityLength.Convert(original, LengthUnit.Feet, LengthUnit.Yards);
        double backToFeet = QuantityLength.Convert(toYards, LengthUnit.Yards, LengthUnit.Feet);

        Assert.IsTrue(Math.Abs(original - backToFeet) < EPSILON);
    }

    [TestMethod]
    public void TestConversion_ZeroValue()
    {
        double result = QuantityLength.Convert(0.0, LengthUnit.Feet, LengthUnit.Inches);
        Assert.AreEqual(0.0, result);
    }

    [TestMethod]
    public void TestConversion_NegativeValue()
    {
        double result = QuantityLength.Convert(-1.0, LengthUnit.Feet, LengthUnit.Inches);
        Assert.IsTrue(Math.Abs(result + 12.0) < EPSILON);
    }

    [TestMethod]
    public void TestConversion_InvalidValue_Throws()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            QuantityLength.Convert(double.NaN, LengthUnit.Feet, LengthUnit.Inches);
        });
    }
}