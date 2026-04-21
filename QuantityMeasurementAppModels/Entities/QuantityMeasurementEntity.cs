using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuantityMeasurementAppModels.Entities
{
    [Table("quantity_measurements")]
    public class QuantityMeasurementEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("operation")]
        public string Operation { get; set; } = string.Empty;

        [Column("first_value")]
        public double FirstValue { get; set; }

        [Column("first_unit")]
        public string? FirstUnit { get; set; }

        [Column("second_value")]
        public double SecondValue { get; set; }

        [Column("second_unit")]
        public string? SecondUnit { get; set; }

        [Column("result_value")]
        public double ResultValue { get; set; }

        [Column("measurement_type")]
        public string? MeasurementType { get; set; }

        [Column("is_error")]
        public bool IsError { get; set; }

        [Column("error_message")]
        public string? ErrorMessage { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Empty constructor
        public QuantityMeasurementEntity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Constructor for two-operand operations: Add, Subtract, Compare, Divide
        public QuantityMeasurementEntity(
            string operation,
            double firstValue, string firstUnit,
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
            CreatedAt       = DateTime.UtcNow;
            UpdatedAt       = DateTime.UtcNow;
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
            CreatedAt       = DateTime.UtcNow;
            UpdatedAt       = DateTime.UtcNow;
        }

        // Constructor for error cases
        public QuantityMeasurementEntity(string operation, string errorMessage)
        {
            Operation    = operation;
            IsError      = true;
            ErrorMessage = errorMessage;
            CreatedAt    = DateTime.UtcNow;
            UpdatedAt    = DateTime.UtcNow;
        }

        public override string ToString()
        {
            if (IsError)
                return "[" + Operation + "] ERROR: " + ErrorMessage;

            return "[" + Operation + "] "
                + FirstValue + " " + FirstUnit
                + " -> " + ResultValue;
        }
    }
}
