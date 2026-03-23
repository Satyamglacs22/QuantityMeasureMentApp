using System;

namespace QuantityMeasurementAppModels.Entity
{
    [Serializable]
    public class QuantityMeasurementEntity
    {
        // Properties
        public string Operation       { get; set; }
        public double FirstValue      { get; set; }
        public string FirstUnit       { get; set; }
        public double SecondValue     { get; set; }
        public string SecondUnit      { get; set; }
        public double ResultValue     { get; set; }
        public string MeasurementType { get; set; }
        public bool   IsError         { get; set; }
        public string ErrorMessage    { get; set; }

        // Parameterless constructor required by JsonSerializer to deserialize from JSON file
        public QuantityMeasurementEntity()
        {
        }

        // Constructor for two-operand operations: Add, Subtract, Compare, Divide
        public QuantityMeasurementEntity(
            string operation,
            double firstValue,  string firstUnit,
            double secondValue, string secondUnit,
            double resultValue,
            string measurementType)
        {
            Operation       = operation;
            FirstValue      = firstValue;
            FirstUnit       = firstUnit;
            SecondValue     = secondValue;
            SecondUnit      = secondUnit;
            ResultValue     = resultValue;
            MeasurementType = measurementType;
            IsError         = false;
        }

        // Constructor for single-operand operations: Convert
        public QuantityMeasurementEntity(
            string operation,
            double inputValue, string inputUnit,
            double resultValue,
            string measurementType)
        {
            Operation       = operation;
            FirstValue      = inputValue;
            FirstUnit       = inputUnit;
            ResultValue     = resultValue;
            MeasurementType = measurementType;
            IsError         = false;
        }

        // Constructor for error cases
        public QuantityMeasurementEntity(string operation, string errorMessage)
        {
            Operation    = operation;
            IsError      = true;
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            if (IsError)
            {
                return "[" + Operation + "] ERROR: " + ErrorMessage;
            }
            return "[" + Operation + "] "
                + FirstValue + " " + FirstUnit
                + " -> " + ResultValue;
        }
    }
}