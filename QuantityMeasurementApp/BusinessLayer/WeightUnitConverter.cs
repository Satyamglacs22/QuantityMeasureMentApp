using System;
using QuantityMeasurementApp.Model;

namespace QuantityMeasurementApp.BusinessLayer;

public static class WeightUnitConverter
{
    public static double ToBase(double value, WeightUnit unit)
    {
        return unit switch
        {
            WeightUnit.Kilogram => value,
            WeightUnit.Gram => value * 0.001,
            WeightUnit.Pound => value * 0.453592,
            _ => throw new ArgumentException("Invalid weight unit")
        };
    }

    public static double FromBase(double baseValue, WeightUnit unit)
    {
        return unit switch
        {
            WeightUnit.Kilogram => baseValue,
            WeightUnit.Gram => baseValue / 0.001,
            WeightUnit.Pound => baseValue / 0.453592,
            _ => throw new ArgumentException("Invalid weight unit")
        };
    }
}