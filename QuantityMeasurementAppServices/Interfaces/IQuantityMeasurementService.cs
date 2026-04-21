using QuantityMeasurementAppModels.DTOs;

namespace QuantityMeasurementAppServices.Interfaces
{
    public interface IQuantityMeasurementService
    {
        // Method for Addition
        QuantityDTO Add(QuantityDTO first, QuantityDTO second, string targetUnit);

        // Method for Subtraction
        QuantityDTO Subtract(QuantityDTO first, QuantityDTO second, string targetUnit);

        // Method for Division
        double Divide(QuantityDTO first, QuantityDTO second);

        // Method for Comparison
        bool Compare(QuantityDTO first, QuantityDTO second);

        // Method for Conversion
        QuantityDTO Convert(QuantityDTO quantity, string targetUnit);
    }
}
