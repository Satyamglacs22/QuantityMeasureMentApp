using System;
using System.ComponentModel.DataAnnotations;
using QuantityMeasurementAppModels.DTOs;
using QuantityMeasurementAppModels.Enums;

namespace QuantityMeasurementAppModels.Validation
{
    // Custom validation attribute to check if UnitName matches MeasurementType
    public class ValidUnitAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Convert object to QuantityDTO
            QuantityDTO? dto = value as QuantityDTO;

            // If DTO is null, skip validation
            if (dto == null)
                return ValidationResult.Success;

            // If required fields are null, skip (handled by [Required])
            if (dto.MeasurementType == null || dto.UnitName == null)
                return ValidationResult.Success;

            string type = dto.MeasurementType.ToLower();
            string unit = dto.UnitName;

            bool isValid = false;

            if (type == "length")
                isValid = Enum.IsDefined(typeof(LengthUnit), unit);
            else if (type == "weight")
                isValid = Enum.IsDefined(typeof(WeightUnit), unit);
            else if (type == "volume")
                isValid = Enum.IsDefined(typeof(VolumeUnit), unit);
            else if (type == "temperature")
                isValid = Enum.IsDefined(typeof(TemperatureUnit), unit);

            if (!isValid)
                return new ValidationResult("Invalid unit for the given measurement type");

            return ValidationResult.Success;
        }
    }
}
