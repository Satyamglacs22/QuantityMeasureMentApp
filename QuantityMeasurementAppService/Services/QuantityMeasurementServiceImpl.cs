using System;
using QuantityMeasurementAppService.Interfaces;
using QuantityMeasurementAppModels.DTO;
using QuantityMeasurementAppModels.Entity;
using QuantityMeasurementAppBusiness.Exceptions;
using QuantityMeasurementAppRepository.Interfaces;
using QuantityMeasurementAppBusiness.Interfaces;
using QuantityMeasurementAppBusiness;
using QuantityMeasurementAppBusiness.Implementations;
using QuantityMeasurementAppModels.Enums;

namespace QuantityMeasurementAppService.Services
{
    public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        private readonly IQuantityMeasurementRepository repository;

        public QuantityMeasurementServiceImpl(IQuantityMeasurementRepository repository)
        {
            this.repository = repository;
        }

        private IMeasurable ResolveUnit(string measurementType, string unitName)
{
    switch (measurementType.ToLower())
    {
        case "length":
            LengthUnit lu = (LengthUnit)Enum.Parse(typeof(LengthUnit), unitName, true);
            return LengthUnitConverter.FromEnum(lu);

        case "weight":
            WeightUnit wu = (WeightUnit)Enum.Parse(typeof(WeightUnit), unitName, true);
            return WeightUnitConverter.FromEnum(wu);

        case "volume":
            VolumeUnit vu = (VolumeUnit)Enum.Parse(typeof(VolumeUnit), unitName, true);
            return VolumeUnitConverter.FromEnum(vu);

        case "temperature":
            TemperatureUnit tu = (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), unitName, true);
            return TemperatureUnitConverter.FromEnum(tu);

        default:
            throw new ArgumentException("Unknown measurement type: " + measurementType);
    }
}
        public QuantityDTO Add(QuantityDTO first, QuantityDTO second, string targetUnit)
        {
            try
            {
                IMeasurable unit1  = ResolveUnit(first.MeasurementType,  first.Unit);
                IMeasurable unit2  = ResolveUnit(second.MeasurementType, second.Unit);
                IMeasurable target = ResolveUnit(first.MeasurementType,  targetUnit);

                Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(first.Value,  unit1);
                Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(second.Value, unit2);

                // Convert both to target unit first then add
                Quantity<IMeasurable> q1Converted = q1.ConvertTo(target);
                Quantity<IMeasurable> q2Converted = q2.ConvertTo(target);
                Quantity<IMeasurable> result = q1Converted.Add(q2Converted);

                repository.Save(new QuantityMeasurementEntity(
                    "Add",
                    first.Value,  first.Unit,
                    second.Value, second.Unit,
                    result.Value,
                    first.MeasurementType));

                return new QuantityDTO(result.Value, targetUnit, first.MeasurementType);
            }
            catch (Exception ex)
            {
                repository.Save(new QuantityMeasurementEntity("Add", ex.Message));
                throw new QuantityMeasurementException("Add operation failed: " + ex.Message, ex);
            }
        }

        public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second, string targetUnit)
        {
            try
            {
                IMeasurable unit1  = ResolveUnit(first.MeasurementType,  first.Unit);
                IMeasurable unit2  = ResolveUnit(second.MeasurementType, second.Unit);
                IMeasurable target = ResolveUnit(first.MeasurementType,  targetUnit);

                Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(first.Value,  unit1);
                Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(second.Value, unit2);

                // Convert both to target unit first then subtract
                Quantity<IMeasurable> q1Converted = q1.ConvertTo(target);
                Quantity<IMeasurable> q2Converted = q2.ConvertTo(target);
                Quantity<IMeasurable> result = q1Converted.Subtract(q2Converted);

                repository.Save(new QuantityMeasurementEntity(
                    "Subtract",
                    first.Value,  first.Unit,
                    second.Value, second.Unit,
                    result.Value,
                    first.MeasurementType));

                return new QuantityDTO(result.Value, targetUnit, first.MeasurementType);
            }
            catch (Exception ex)
            {
                repository.Save(new QuantityMeasurementEntity("Subtract", ex.Message));
                throw new QuantityMeasurementException("Subtract operation failed: " + ex.Message, ex);
            }
        }

        public double Divide(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                IMeasurable unit1 = ResolveUnit(first.MeasurementType,  first.Unit);
                IMeasurable unit2 = ResolveUnit(second.MeasurementType, second.Unit);

                Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(first.Value,  unit1);
                Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(second.Value, unit2);
                double result = q1.Divide(q2);

                repository.Save(new QuantityMeasurementEntity(
                    "Divide",
                    first.Value,  first.Unit,
                    second.Value, second.Unit,
                    result,
                    first.MeasurementType));

                return result;
            }
            catch (Exception ex)
            {
                repository.Save(new QuantityMeasurementEntity("Divide", ex.Message));
                throw new QuantityMeasurementException("Divide operation failed: " + ex.Message, ex);
            }
        }

        public bool Compare(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                IMeasurable unit1 = ResolveUnit(first.MeasurementType,  first.Unit);
                IMeasurable unit2 = ResolveUnit(second.MeasurementType, second.Unit);

                Quantity<IMeasurable> q1 = new Quantity<IMeasurable>(first.Value,  unit1);
                Quantity<IMeasurable> q2 = new Quantity<IMeasurable>(second.Value, unit2);
                bool result = q1.Equals(q2);

                repository.Save(new QuantityMeasurementEntity(
                    "Compare",
                    first.Value,  first.Unit,
                    second.Value, second.Unit,
                    result ? 1 : 0,
                    first.MeasurementType));

                return result;
            }
            catch (Exception ex)
            {
                repository.Save(new QuantityMeasurementEntity("Compare", ex.Message));
                throw new QuantityMeasurementException("Compare operation failed: " + ex.Message, ex);
            }
        }

        public QuantityDTO Convert(QuantityDTO quantity, string targetUnit)
        {
            try
            {
                IMeasurable unit   = ResolveUnit(quantity.MeasurementType, quantity.Unit);
                IMeasurable target = ResolveUnit(quantity.MeasurementType, targetUnit);

                Quantity<IMeasurable> q      = new Quantity<IMeasurable>(quantity.Value, unit);
                Quantity<IMeasurable> result = q.ConvertTo(target);

                repository.Save(new QuantityMeasurementEntity(
                    "Convert",
                    quantity.Value, quantity.Unit,
                    result.Value,
                    quantity.MeasurementType));

                return new QuantityDTO(result.Value, targetUnit, quantity.MeasurementType);
            }
            catch (Exception ex)
            {
                repository.Save(new QuantityMeasurementEntity("Convert", ex.Message));
                throw new QuantityMeasurementException("Convert operation failed: " + ex.Message, ex);
            }
        }
    }
}


