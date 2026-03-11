using System;

public static class LengthUnitConverter
{
    public static double ToBase(double value, LengthUnit unit)
    {
        switch (unit)
        {
            case LengthUnit.Feet:
                return value;

            case LengthUnit.Inches:
                return value / 12.0;

            case LengthUnit.Yards:
                return value * 3.0;

            case LengthUnit.Centimeters:
                return value / 30.48;

            default:
                throw new ArgumentException("Invalid unit");
        }
    }

    public static double FromBase(double baseValue, LengthUnit unit)
    {
        switch (unit)
        {
            case LengthUnit.Feet:
                return baseValue;

            case LengthUnit.Inches:
                return baseValue * 12.0;

            case LengthUnit.Yards:
                return baseValue / 3.0;

            case LengthUnit.Centimeters:
                return baseValue * 30.48;

            default:
                throw new ArgumentException("Invalid unit");
        }
    }
}