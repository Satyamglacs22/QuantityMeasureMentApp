using System;

public class QuantityLength
{
    private readonly double value;
    private readonly LengthUnit unit;

    private const double EPSILON = 0.000001;

    public QuantityLength(double value, LengthUnit unit)
    {
        if (unit == null)
            throw new ArgumentException("Unit cannot be null");

        if (!double.IsFinite(value))
            throw new ArgumentException("Invalid numeric value");

        this.value = value;
        this.unit = unit;
    }

    public double Value => value;
    public LengthUnit Unit => unit;

    // Convert current quantity to base unit (feet)
    private double ToBase()
    {
        return LengthUnitConverter.ToBase(value, unit);
    }

    // Convert current quantity to another unit
    public QuantityLength ConvertTo(LengthUnit targetUnit)
    {
        if (targetUnit == null)
            throw new ArgumentException("Target unit cannot be null");

        double baseValue = ToBase();
        double convertedValue = LengthUnitConverter.FromBase(baseValue, targetUnit);

        return new QuantityLength(convertedValue, targetUnit);
    }

    // UC6: Addition with default unit (first operand unit)
    public QuantityLength Add(QuantityLength other)
    {
        return Add(other, this.unit);
    }

    // UC7: Addition with explicit target unit
    public QuantityLength Add(QuantityLength other, LengthUnit targetUnit)
    {
        if (other == null)
            throw new ArgumentException("Other quantity cannot be null");

        if (targetUnit == null)
            throw new ArgumentException("Target unit cannot be null");

        double sumBase = this.ToBase() + other.ToBase();

        double resultValue = LengthUnitConverter.FromBase(sumBase, targetUnit);

        return new QuantityLength(resultValue, targetUnit);
    }

    // Static method used by some test cases
    public static QuantityLength Add(QuantityLength a, QuantityLength b, LengthUnit targetUnit)
    {
        if (a == null || b == null)
            throw new ArgumentException("Operands cannot be null");

        return a.Add(b, targetUnit);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not QuantityLength other)
            return false;

        double diff = Math.Abs(this.ToBase() - other.ToBase());

        return diff < EPSILON;
    }

    public override int GetHashCode()
    {
        return Math.Round(ToBase(), 6).GetHashCode();
    }

    public override string ToString()
    {
        return $"Quantity({value}, {unit})";
    }
}