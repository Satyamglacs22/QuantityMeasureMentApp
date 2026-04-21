using System.Collections.Generic;
using QuantityMeasurementAppModels.Entities;

namespace QuantityMeasurementAppModels.DTOs
{
    public class QuantityMeasurementResponseDTO
    {
        public double  ThisValue            { get; set; }
        public string? ThisUnit             { get; set; }
        public string? ThisMeasurementType  { get; set; }
        public double  ThatValue            { get; set; }
        public string? ThatUnit             { get; set; }
        public string? ThatMeasurementType  { get; set; }
        public string? Operation            { get; set; }
        public string? ResultString         { get; set; }
        public double  ResultValue          { get; set; }
        public string? ResultUnit           { get; set; }
        public string? ResultMeasurementType{ get; set; }
        public string? ErrorMessage         { get; set; }
        public bool    IsError              { get; set; }

        // Convert one entity to one DTO
        public static QuantityMeasurementResponseDTO FromEntity(QuantityMeasurementEntity entity)
        {
            return new QuantityMeasurementResponseDTO
            {
                ThisValue           = entity.FirstValue,
                ThisUnit            = entity.FirstUnit,
                ThisMeasurementType = entity.MeasurementType,
                ThatValue           = entity.SecondValue,
                ThatUnit            = entity.SecondUnit,
                ThatMeasurementType = entity.MeasurementType,
                Operation           = entity.Operation,
                ResultValue         = entity.ResultValue,
                ErrorMessage        = entity.ErrorMessage,
                IsError             = entity.IsError
            };
        }

        // Convert a list of entities to a list of DTOs
        public static List<QuantityMeasurementResponseDTO> FromEntityList(List<QuantityMeasurementEntity> entities)
        {
            List<QuantityMeasurementResponseDTO> list = new();
            foreach (QuantityMeasurementEntity entity in entities)
                list.Add(FromEntity(entity));
            return list;
        }

        // Convert this DTO back to an Entity
        public QuantityMeasurementEntity ToEntity()
        {
            if (IsError)
                return new QuantityMeasurementEntity(Operation ?? string.Empty, ErrorMessage ?? string.Empty);

            return new QuantityMeasurementEntity(
                Operation           ?? string.Empty,
                ThisValue,           ThisUnit        ?? string.Empty,
                ThatValue,           ThatUnit        ?? string.Empty,
                ResultValue,
                ThisMeasurementType ?? string.Empty);
        }

        // Convert a list of DTOs to a list of entities
        public static List<QuantityMeasurementEntity> ToEntityList(List<QuantityMeasurementResponseDTO> dtos)
        {
            List<QuantityMeasurementEntity> list = new();
            foreach (QuantityMeasurementResponseDTO dto in dtos)
                list.Add(dto.ToEntity());
            return list;
        }
    }
}
