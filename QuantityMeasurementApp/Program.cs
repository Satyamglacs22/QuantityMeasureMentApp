using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("convert(1.0, FEET, INCHES) → " +
            QuantityLength.Convert(1.0, LengthUnit.Feet, LengthUnit.Inches));

        Console.WriteLine("convert(3.0, YARDS, FEET) → " +
            QuantityLength.Convert(3.0, LengthUnit.Yards, LengthUnit.Feet));

        Console.WriteLine("convert(36.0, INCHES, YARDS) → " +
            QuantityLength.Convert(36.0, LengthUnit.Inches, LengthUnit.Yards));

        Console.WriteLine("convert(2.54, CENTIMETERS, INCHES) → " +
            QuantityLength.Convert(2.54, LengthUnit.Centimeters, LengthUnit.Inches));

        Console.ReadLine();
    }
}