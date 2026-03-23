using System;
using QuantityMeasurementApp.Controller;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementAppModels.DTO;

namespace QuantityMeasurementApp.Menu
{
    public class QuantityMenu : IMenu
    {
        private QuantityMeasurementController controller;

        public QuantityMenu(QuantityMeasurementController controller)
        {
            this.controller = controller;
        }

        public void Run()
        {
            // Infinite loop until user chooses to exit
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║      QUANTITY MEASUREMENT APPLICATION    ║");
                Console.WriteLine("╠══════════════════════════════════════════╣");
                Console.WriteLine("║  LENGTH                                  ║");
                Console.WriteLine("║   1.  Compare Two Lengths                ║");
                Console.WriteLine("║   2.  Convert Length                     ║");
                Console.WriteLine("║   3.  Add Two Lengths                    ║");
                Console.WriteLine("║   4.  Subtract Two Lengths               ║");
                Console.WriteLine("║   5.  Divide Two Lengths                 ║");
                Console.WriteLine("╠══════════════════════════════════════════╣");
                Console.WriteLine("║  WEIGHT                                  ║");
                Console.WriteLine("║   6.  Compare Two Weights                ║");
                Console.WriteLine("║   7.  Convert Weight                     ║");
                Console.WriteLine("║   8.  Add Two Weights                    ║");
                Console.WriteLine("║   9.  Subtract Two Weights               ║");
                Console.WriteLine("║   10. Divide Two Weights                 ║");
                Console.WriteLine("╠══════════════════════════════════════════╣");
                Console.WriteLine("║  VOLUME                                  ║");
                Console.WriteLine("║   11. Compare Two Volumes                ║");
                Console.WriteLine("║   12. Convert Volume                     ║");
                Console.WriteLine("║   13. Add Two Volumes                    ║");
                Console.WriteLine("║   14. Subtract Two Volumes               ║");
                Console.WriteLine("║   15. Divide Two Volumes                 ║");
                Console.WriteLine("╠══════════════════════════════════════════╣");
                Console.WriteLine("║  TEMPERATURE                             ║");
                Console.WriteLine("║   16. Compare Two Temperatures           ║");
                Console.WriteLine("║   17. Convert Temperature                ║");
                Console.WriteLine("║   18. Try Temperature Arithmetic         ║");
                Console.WriteLine("╠══════════════════════════════════════════╣");
                Console.WriteLine("║   0.  Exit                               ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.Write("  Enter Your Choice: ");

                int choice = int.Parse(Console.ReadLine());

                if (choice == 0)
                {
                    Console.WriteLine("\n  Goodbye! Exiting application...");
                    return;
                }
                else if (choice == 1)
                {
                    Console.WriteLine("\n--- Compare Two Lengths ---");
                    QuantityDTO first  = ReadQuantity("First",  "Length", "Feet/Inches/Centimeters/Yards");
                    QuantityDTO second = ReadQuantity("Second", "Length", "Feet/Inches/Centimeters/Yards");
                    controller.PerformLengthComparison(first, second);
                }
                else if (choice == 2)
                {
                    Console.WriteLine("\n--- Convert Length ---");
                    QuantityDTO quantity = ReadQuantity("", "Length", "Feet/Inches/Centimeters/Yards");
                    Console.Write("  Enter Target Unit (Feet/Inches/Centimeters/Yards): ");
                    string targetUnit = Console.ReadLine();
                    controller.PerformLengthConversion(quantity, targetUnit);
                }
                else if (choice == 3)
                {
                    Console.WriteLine("\n--- Add Two Lengths ---");
                    QuantityDTO first  = ReadQuantity("First",  "Length", "Feet/Inches/Centimeters/Yards");
                    QuantityDTO second = ReadQuantity("Second", "Length", "Feet/Inches/Centimeters/Yards");
                    Console.Write("  Enter Target Unit (Feet/Inches/Centimeters/Yards): ");
                    string targetUnit = Console.ReadLine();
                    controller.PerformLengthAddition(first, second, targetUnit);
                }
                else if (choice == 4)
                {
                    Console.WriteLine("\n--- Subtract Two Lengths ---");
                    QuantityDTO first  = ReadQuantity("First",  "Length", "Feet/Inches/Centimeters/Yards");
                    QuantityDTO second = ReadQuantity("Second", "Length", "Feet/Inches/Centimeters/Yards");
                    Console.Write("  Enter Target Unit (Feet/Inches/Centimeters/Yards): ");
                    string targetUnit = Console.ReadLine();
                    controller.PerformLengthSubtraction(first, second, targetUnit);
                }
                else if (choice == 5)
                {
                    Console.WriteLine("\n--- Divide Two Lengths ---");
                    QuantityDTO first  = ReadQuantity("First",  "Length", "Feet/Inches/Centimeters/Yards");
                    QuantityDTO second = ReadQuantity("Second", "Length", "Feet/Inches/Centimeters/Yards");
                    controller.PerformLengthDivision(first, second);
                }
                else if (choice == 6)
                {
                    Console.WriteLine("\n--- Compare Two Weights ---");
                    QuantityDTO first  = ReadQuantity("First",  "Weight", "Kilogram/Gram/Pound");
                    QuantityDTO second = ReadQuantity("Second", "Weight", "Kilogram/Gram/Pound");
                    controller.PerformWeightComparison(first, second);
                }
                else if (choice == 7)
                {
                    Console.WriteLine("\n--- Convert Weight ---");
                    QuantityDTO quantity = ReadQuantity("", "Weight", "Kilogram/Gram/Pound");
                    Console.Write("  Enter Target Unit (Kilogram/Gram/Pound): ");
                    string targetUnit = Console.ReadLine();
                    controller.PerformWeightConversion(quantity, targetUnit);
                }
                else if (choice == 8)
                {
                    Console.WriteLine("\n--- Add Two Weights ---");
                    QuantityDTO first  = ReadQuantity("First",  "Weight", "Kilogram/Gram/Pound");
                    QuantityDTO second = ReadQuantity("Second", "Weight", "Kilogram/Gram/Pound");
                    Console.Write("  Enter Target Unit (Kilogram/Gram/Pound): ");
                    string targetUnit = Console.ReadLine();
                    controller.PerformWeightAddition(first, second, targetUnit);
                }
                else if (choice == 9)
                {
                    Console.WriteLine("\n--- Subtract Two Weights ---");
                    QuantityDTO first  = ReadQuantity("First",  "Weight", "Kilogram/Gram/Pound");
                    QuantityDTO second = ReadQuantity("Second", "Weight", "Kilogram/Gram/Pound");
                    Console.Write("  Enter Target Unit (Kilogram/Gram/Pound): ");
                    string targetUnit = Console.ReadLine();
                    controller.PerformWeightSubtraction(first, second, targetUnit);
                }
                else if (choice == 10)
                {
                    Console.WriteLine("\n--- Divide Two Weights ---");
                    QuantityDTO first  = ReadQuantity("First",  "Weight", "Kilogram/Gram/Pound");
                    QuantityDTO second = ReadQuantity("Second", "Weight", "Kilogram/Gram/Pound");
                    controller.PerformWeightDivision(first, second);
                }
                else if (choice == 11)
                {
                    Console.WriteLine("\n--- Compare Two Volumes ---");
                    QuantityDTO first  = ReadQuantity("First",  "Volume", "Litre/Millilitre/Gallon");
                    QuantityDTO second = ReadQuantity("Second", "Volume", "Litre/Millilitre/Gallon");
                    controller.PerformVolumeComparison(first, second);
                }
                else if (choice == 12)
                {
                    Console.WriteLine("\n--- Convert Volume ---");
                    QuantityDTO quantity = ReadQuantity("", "Volume", "Litre/Millilitre/Gallon");
                    Console.Write("  Enter Target Unit (Litre/Millilitre/Gallon): ");
                    string targetUnit = Console.ReadLine();
                    controller.PerformVolumeConversion(quantity, targetUnit);
                }
                else if (choice == 13)
                {
                    Console.WriteLine("\n--- Add Two Volumes ---");
                    QuantityDTO first  = ReadQuantity("First",  "Volume", "Litre/Millilitre/Gallon");
                    QuantityDTO second = ReadQuantity("Second", "Volume", "Litre/Millilitre/Gallon");
                    Console.Write("  Enter Target Unit (Litre/Millilitre/Gallon): ");
                    string targetUnit = Console.ReadLine();
                    controller.PerformVolumeAddition(first, second, targetUnit);
                }
                else if (choice == 14)
                {
                    Console.WriteLine("\n--- Subtract Two Volumes ---");
                    QuantityDTO first  = ReadQuantity("First",  "Volume", "Litre/Millilitre/Gallon");
                    QuantityDTO second = ReadQuantity("Second", "Volume", "Litre/Millilitre/Gallon");
                    Console.Write("  Enter Target Unit (Litre/Millilitre/Gallon): ");
                    string targetUnit = Console.ReadLine();
                    controller.PerformVolumeSubtraction(first, second, targetUnit);
                }
                else if (choice == 15)
                {
                    Console.WriteLine("\n--- Divide Two Volumes ---");
                    QuantityDTO first  = ReadQuantity("First",  "Volume", "Litre/Millilitre/Gallon");
                    QuantityDTO second = ReadQuantity("Second", "Volume", "Litre/Millilitre/Gallon");
                    controller.PerformVolumeDivision(first, second);
                }
                else if (choice == 16)
                {
                    Console.WriteLine("\n--- Compare Two Temperatures ---");
                    QuantityDTO first  = ReadQuantity("First",  "Temperature", "Celsius/Fahrenheit/Kelvin");
                    QuantityDTO second = ReadQuantity("Second", "Temperature", "Celsius/Fahrenheit/Kelvin");
                    controller.PerformTemperatureComparison(first, second);
                }
                else if (choice == 17)
                {
                    Console.WriteLine("\n--- Convert Temperature ---");
                    QuantityDTO quantity = ReadQuantity("", "Temperature", "Celsius/Fahrenheit/Kelvin");
                    Console.Write("  Enter Target Unit (Celsius/Fahrenheit/Kelvin): ");
                    string targetUnit = Console.ReadLine();
                    controller.PerformTemperatureConversion(quantity, targetUnit);
                }
                else if (choice == 18)
                {
                    Console.WriteLine("\n--- Try Temperature Arithmetic ---");
                    Console.WriteLine("  Note: Temperature arithmetic is not supported.");
                    controller.PerformTemperatureArithmetic(
                        new QuantityDTO(100, "Celsius", "Temperature"),
                        new QuantityDTO(50,  "Celsius", "Temperature"),
                        "Celsius");
                }
                else
                {
                    Console.WriteLine("\n  Invalid Choice. Please enter a number between 0 and 18.");
                }
            }
        }

        // Helper method to read a QuantityDTO from the user
        private QuantityDTO ReadQuantity(string label, string measurementType, string unitHint)
        {
            string prefix = label != "" ? label + " " : "";

            Console.Write("  Enter " + prefix + "Value: ");
            double value = double.Parse(Console.ReadLine());

            Console.Write("  Enter " + prefix + "Unit (" + unitHint + "): ");
            string unit = Console.ReadLine();

            return new QuantityDTO(value, unit, measurementType);
        }
    }
}
