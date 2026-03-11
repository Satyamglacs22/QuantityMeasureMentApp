using System;

public class Program
{
    public static void Main()
    {
        var q1 = new QuantityLength(1.0, LengthUnit.Feet);
        var q2 = new QuantityLength(12.0, LengthUnit.Inches);

        Console.WriteLine("Convert:");
        Console.WriteLine(q1.ConvertTo(LengthUnit.Inches));

        Console.WriteLine();

        Console.WriteLine("Addition (UC6):");
        Console.WriteLine(q1.Add(q2));

        Console.WriteLine();

        Console.WriteLine("Addition with Target Unit (UC7):");
        Console.WriteLine(q1.Add(q2, LengthUnit.Yards));

        Console.WriteLine();

        Console.WriteLine("Equality:");
        var q3 = new QuantityLength(36.0, LengthUnit.Inches);
        var q4 = new QuantityLength(1.0, LengthUnit.Yards);
        Console.WriteLine(q3.Equals(q4));

        Console.ReadLine();
    }
}