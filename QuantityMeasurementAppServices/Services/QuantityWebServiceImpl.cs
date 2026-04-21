using QuantityMeasurementAppModels.DTOs;
using QuantityMeasurementAppRepositories.Interfaces;
using QuantityMeasurementAppServices.Interfaces;

namespace QuantityMeasurementAppServices.Services
{
    public class QuantityWebServiceImpl : IQuantityWebService
    {
        private readonly IQuantityMeasurementService service;
        private readonly IQuantityRecordRepository   repository;

        // Constructor — DI injects service and repository
        public QuantityWebServiceImpl(
            IQuantityMeasurementService service,
            IQuantityRecordRepository   repository)
        {
            this.service    = service;
            this.repository = repository;
        }

        // Compare two quantities
        public QuantityMeasurementResponseDTO Compare(QuantityInputRequest request)
        {
            bool result = service.Compare(request.ThisQuantityDTO, request.ThatQuantityDTO);

            return new QuantityMeasurementResponseDTO
            {
                ThisValue           = request.ThisQuantityDTO.Value,
                ThisUnit            = request.ThisQuantityDTO.UnitName,
                ThisMeasurementType = request.ThisQuantityDTO.MeasurementType,
                ThatValue           = request.ThatQuantityDTO.Value,
                ThatUnit            = request.ThatQuantityDTO.UnitName,
                ThatMeasurementType = request.ThatQuantityDTO.MeasurementType,
                Operation           = "Compare",
                ResultString        = result.ToString(),
                IsError             = false
            };
        }

        // Convert a quantity to a target unit
        public QuantityMeasurementResponseDTO Convert(ConvertRequest request)
        {
            QuantityDTO result = service.Convert(request.ThisQuantityDTO, request.TargetUnit);

            return new QuantityMeasurementResponseDTO
            {
                ThisValue           = request.ThisQuantityDTO.Value,
                ThisUnit            = request.ThisQuantityDTO.UnitName,
                ThisMeasurementType = request.ThisQuantityDTO.MeasurementType,
                Operation           = "Convert",
                ResultValue         = result.Value,
                ResultUnit          = request.TargetUnit,
                IsError             = false
            };
        }

        // Add two quantities
        public QuantityMeasurementResponseDTO Add(ArithmeticRequest request)
        {
            QuantityDTO result = service.Add(
                request.ThisQuantityDTO,
                request.ThatQuantityDTO,
                request.TargetUnit);

            return new QuantityMeasurementResponseDTO
            {
                ThisValue            = request.ThisQuantityDTO.Value,
                ThisUnit             = request.ThisQuantityDTO.UnitName,
                ThisMeasurementType  = request.ThisQuantityDTO.MeasurementType,
                ThatValue            = request.ThatQuantityDTO.Value,
                ThatUnit             = request.ThatQuantityDTO.UnitName,
                ThatMeasurementType  = request.ThatQuantityDTO.MeasurementType,
                Operation            = "Add",
                ResultValue          = result.Value,
                ResultUnit           = request.TargetUnit,
                ResultMeasurementType = request.ThisQuantityDTO.MeasurementType,
                IsError              = false
            };
        }

        // Subtract two quantities
        public QuantityMeasurementResponseDTO Subtract(ArithmeticRequest request)
        {
            QuantityDTO result = service.Subtract(
                request.ThisQuantityDTO,
                request.ThatQuantityDTO,
                request.TargetUnit);

            return new QuantityMeasurementResponseDTO
            {
                ThisValue            = request.ThisQuantityDTO.Value,
                ThisUnit             = request.ThisQuantityDTO.UnitName,
                ThisMeasurementType  = request.ThisQuantityDTO.MeasurementType,
                ThatValue            = request.ThatQuantityDTO.Value,
                ThatUnit             = request.ThatQuantityDTO.UnitName,
                ThatMeasurementType  = request.ThatQuantityDTO.MeasurementType,
                Operation            = "Subtract",
                ResultValue          = result.Value,
                ResultUnit           = request.TargetUnit,
                ResultMeasurementType = request.ThisQuantityDTO.MeasurementType,
                IsError              = false
            };
        }

        // Divide two quantities
        public QuantityMeasurementResponseDTO Divide(QuantityInputRequest request)
        {
            double result = service.Divide(request.ThisQuantityDTO, request.ThatQuantityDTO);

            return new QuantityMeasurementResponseDTO
            {
                ThisValue           = request.ThisQuantityDTO.Value,
                ThisUnit            = request.ThisQuantityDTO.UnitName,
                ThisMeasurementType = request.ThisQuantityDTO.MeasurementType,
                ThatValue           = request.ThatQuantityDTO.Value,
                ThatUnit            = request.ThatQuantityDTO.UnitName,
                ThatMeasurementType = request.ThatQuantityDTO.MeasurementType,
                Operation           = "Divide",
                ResultValue         = result,
                IsError             = false
            };
        }

        // Get history filtered by operation
        public List<QuantityMeasurementResponseDTO> GetHistoryByOperation(string operation)
        {
            return QuantityMeasurementResponseDTO.FromEntityList(
                repository.GetByOperation(operation));
        }

        // Get history filtered by measurement type
        public List<QuantityMeasurementResponseDTO> GetHistoryByType(string measurementType)
        {
            return QuantityMeasurementResponseDTO.FromEntityList(
                repository.GetByMeasurementType(measurementType));
        }

        // Get all error records
        public List<QuantityMeasurementResponseDTO> GetErrorHistory()
        {
            return QuantityMeasurementResponseDTO.FromEntityList(
                repository.GetErrorHistory());
        }

        // Get count of successful operations by type
        public int GetOperationCount(string operation)
        {
            return repository.GetOperationCount(operation);
        }
    }
}
