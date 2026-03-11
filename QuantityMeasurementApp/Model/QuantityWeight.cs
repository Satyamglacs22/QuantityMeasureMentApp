using System;
using QuantityMeasurementApp.BusinessLayer;

namespace QuantityMeasurementApp.Model;

public class QuantityWeight
{
    private readonly double value;
    private readonly WeightUnit unit;

    private const double EPSILON = 0.0001;

    public QuantityWeight(double value, WeightUnit unit)
    {
        if (!double.IsFinite(value))
            throw new ArgumentException("Invalid value");

        this.value = value;
        this.unit = unit;
    }

    public double Value => value;
    public WeightUnit Unit => unit;

    public QuantityWeight ConvertTo(WeightUnit targetUnit)
    {
        double baseValue = WeightUnitConverter.ToBase(value, unit);
        double convertedValue = WeightUnitConverter.FromBase(baseValue, targetUnit);

        return new QuantityWeight(convertedValue, targetUnit);
    }

    public QuantityWeight Add(QuantityWeight other)
    {
        return Add(other, this.unit);
    }

    public QuantityWeight Add(QuantityWeight other, WeightUnit targetUnit)
    {
        double thisBase = WeightUnitConverter.ToBase(this.value, this.unit);
        double otherBase = WeightUnitConverter.ToBase(other.value, other.unit);

        double sumBase = thisBase + otherBase;

        double result = WeightUnitConverter.FromBase(sumBase, targetUnit);

        return new QuantityWeight(result, targetUnit);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not QuantityWeight other)
            return false;

        double thisBase = WeightUnitConverter.ToBase(this.value, this.unit);
        double otherBase = WeightUnitConverter.ToBase(other.value, other.unit);

        return Math.Abs(thisBase - otherBase) < EPSILON;
    }

    public override int GetHashCode()
    {
        return WeightUnitConverter.ToBase(value, unit).GetHashCode();
    }
}