using System.ComponentModel.DataAnnotations;
using QuantityMeasurementAppModels.Validation;

namespace QuantityMeasurementAppModels.DTOs
{
    [ValidUnit]
    public class QuantityDTO
    {
        [Required(ErrorMessage = "Value is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be non-negative")]
        public double Value { get; set; }

        [Required(ErrorMessage = "Unit name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Unit name must contain only letters")]
        public string UnitName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Measurement type is required")]
        [RegularExpression("^(Length|Weight|Volume|Temperature)$",
            ErrorMessage = "Measurement type must be Length, Weight, Volume or Temperature")]
        public string MeasurementType { get; set; } = string.Empty;

        // Constructor
        public QuantityDTO(double value, string unitName, string measurementType)
        {
            Value           = value;
            UnitName        = unitName;
            MeasurementType = measurementType;
        }

        // Empty Constructor
        public QuantityDTO() { }

        public override string ToString()
        {
            return Value + " " + UnitName;
        }
    }
}
