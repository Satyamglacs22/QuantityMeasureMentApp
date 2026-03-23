using System;
using QuantityMeasurementAppModels.DTO;
using QuantityMeasurementAppService.Interfaces;

namespace QuantityMeasurementApp.Controller
{
    public class QuantityMeasurementController
    {
        private IQuantityMeasurementService service;

        public QuantityMeasurementController(IQuantityMeasurementService service)
        {
            if (service == null)
                throw new ArgumentException("Service cannot be null");
            this.service = service;
        }

        public void PerformLengthComparison(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                bool result = service.Compare(first, second);
                Console.WriteLine(result ? "Both Lengths Are Equal" : "Lengths Are Not Equal");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformLengthConversion(QuantityDTO quantity, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Convert(quantity, targetUnit);
                Console.WriteLine(quantity + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformLengthAddition(QuantityDTO first, QuantityDTO second, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Add(first, second, targetUnit);
                Console.WriteLine(first + " + " + second + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformLengthSubtraction(QuantityDTO first, QuantityDTO second, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Subtract(first, second, targetUnit);
                Console.WriteLine(first + " - " + second + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformLengthDivision(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                double result = service.Divide(first, second);
                Console.WriteLine(first + " / " + second + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformWeightComparison(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                bool result = service.Compare(first, second);
                Console.WriteLine(result ? "Weights Are Equal" : "Weights Are Not Equal");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformWeightConversion(QuantityDTO quantity, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Convert(quantity, targetUnit);
                Console.WriteLine(quantity + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformWeightAddition(QuantityDTO first, QuantityDTO second, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Add(first, second, targetUnit);
                Console.WriteLine(first + " + " + second + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformWeightSubtraction(QuantityDTO first, QuantityDTO second, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Subtract(first, second, targetUnit);
                Console.WriteLine(first + " - " + second + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformWeightDivision(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                double result = service.Divide(first, second);
                Console.WriteLine(first + " / " + second + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformVolumeComparison(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                bool result = service.Compare(first, second);
                Console.WriteLine(result ? "Volumes Are Equal" : "Volumes Are Not Equal");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformVolumeConversion(QuantityDTO quantity, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Convert(quantity, targetUnit);
                Console.WriteLine(quantity + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformVolumeAddition(QuantityDTO first, QuantityDTO second, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Add(first, second, targetUnit);
                Console.WriteLine(first + " + " + second + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformVolumeSubtraction(QuantityDTO first, QuantityDTO second, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Subtract(first, second, targetUnit);
                Console.WriteLine(first + " - " + second + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformVolumeDivision(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                double result = service.Divide(first, second);
                Console.WriteLine(first + " / " + second + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformTemperatureComparison(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                bool result = service.Compare(first, second);
                Console.WriteLine(result ? "Temperatures Are Equal" : "Temperatures Are Not Equal");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformTemperatureConversion(QuantityDTO quantity, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Convert(quantity, targetUnit);
                Console.WriteLine(quantity + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        public void PerformTemperatureArithmetic(QuantityDTO first, QuantityDTO second, string targetUnit)
        {
            try
            {
                QuantityDTO result = service.Add(first, second, targetUnit);
                Console.WriteLine(first + " + " + second + " = " + result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
    }
}