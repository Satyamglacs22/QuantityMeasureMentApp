using System;

public class QuantityLength
{
    private readonly double value;
    private readonly LengthUnit unit;

    private const double EPSILON = 0.000001;

    public QuantityLength(double value, LengthUnit unit)
    {
        if (!double.IsFinite(value))
            throw new ArgumentException("Invalid numeric value");

        if (!Enum.IsDefined(typeof(LengthUnit), unit))
            throw new ArgumentException("Invalid unit");

        this.value = value;
        this.unit = unit;
    }

    public double Value => value;
    public LengthUnit Unit => unit;

    // ---------------- STATIC CONVERSION ----------------

    public static double Convert(double value, LengthUnit source, LengthUnit target)
    {
        if (!double.IsFinite(value))
            throw new ArgumentException("Invalid numeric value");

        if (!Enum.IsDefined(typeof(LengthUnit), source) ||
            !Enum.IsDefined(typeof(LengthUnit), target))
            throw new ArgumentException("Invalid unit");

        double valueInFeet = NormalizeToFeet(value, source);
        return ConvertFromFeet(valueInFeet, target);
    }

    // ---------------- INSTANCE CONVERSION ----------------

    public QuantityLength ConvertTo(LengthUnit targetUnit)
    {
        double convertedValue = Convert(this.value, this.unit, targetUnit);
        return new QuantityLength(convertedValue, targetUnit);
    }

    // ---------------- PRIVATE HELPERS ----------------

    private static double NormalizeToFeet(double value, LengthUnit unit)
    {
        return unit switch
        {
            LengthUnit.Feet => value,
            LengthUnit.Inches => value / 12.0,
            LengthUnit.Yards => value * 3.0,
            LengthUnit.Centimeters => value / 30.48,
            LengthUnit.Meters => value * 3.28084,
            _ => throw new ArgumentException("Unsupported unit")
        };
    }

    private static double ConvertFromFeet(double valueInFeet, LengthUnit unit)
    {
        return unit switch
        {
            LengthUnit.Feet => valueInFeet,
            LengthUnit.Inches => valueInFeet * 12.0,
            LengthUnit.Yards => valueInFeet / 3.0,
            LengthUnit.Centimeters => valueInFeet * 30.48,
            LengthUnit.Meters => valueInFeet / 3.28084,
            _ => throw new ArgumentException("Unsupported unit")
        };
    }

    // ---------------- EQUALITY ----------------

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not QuantityLength other)
            return false;

        double thisFeet = NormalizeToFeet(this.value, this.unit);
        double otherFeet = NormalizeToFeet(other.value, other.unit);

        return Math.Abs(thisFeet - otherFeet) < EPSILON;
    }

    public override int GetHashCode()
    {
        return NormalizeToFeet(this.value, this.unit).GetHashCode();
    }

    public override string ToString()
    {
        return $"Quantity({value}, {unit})";
    }
}