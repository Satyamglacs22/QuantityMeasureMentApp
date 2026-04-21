using QuantityMeasurementAppModels.DTOs;

namespace QuantityMeasurementAppServices.Interfaces
{
    public interface IQuantityWebService
    {
        QuantityMeasurementResponseDTO Compare(QuantityInputRequest request);
        QuantityMeasurementResponseDTO Convert(ConvertRequest request);
        QuantityMeasurementResponseDTO Add(ArithmeticRequest request);
        QuantityMeasurementResponseDTO Subtract(ArithmeticRequest request);
        QuantityMeasurementResponseDTO Divide(QuantityInputRequest request);

        List<QuantityMeasurementResponseDTO> GetHistoryByOperation(string operation);
        List<QuantityMeasurementResponseDTO> GetHistoryByType(string measurementType);
        List<QuantityMeasurementResponseDTO> GetErrorHistory();
        int GetOperationCount(string operation);
    }
}
