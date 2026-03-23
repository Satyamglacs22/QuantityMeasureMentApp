using QuantityMeasurementAppModels.Enums;
using QuantityMeasurementAppBusiness.Interfaces;

namespace QuantityMeasurementAppBusiness.Implementations
{
    public sealed class WeightUnitConverter : IMeasurable
    {
        private readonly double _factor;
        private readonly WeightUnit _unit;

        private WeightUnitConverter(double factor, WeightUnit unit)
        {
            _factor = factor;
            _unit = unit;
        }

        public static readonly WeightUnitConverter Kilogram = new WeightUnitConverter(1.0,      WeightUnit.Kilogram);
        public static readonly WeightUnitConverter Gram     = new WeightUnitConverter(0.001,    WeightUnit.Gram);
        public static readonly WeightUnitConverter Pound    = new WeightUnitConverter(0.453592, WeightUnit.Pound);

        public static WeightUnitConverter FromEnum(WeightUnit unit)
        {
            if (unit == WeightUnit.Kilogram) return Kilogram;
            if (unit == WeightUnit.Gram)     return Gram;
            if (unit == WeightUnit.Pound)    return Pound;
            throw new ArgumentException("Unknown WeightUnit: " + unit);
        }

        public double ConvertToBaseUnit(double value)
        {
            return value * _factor;
        }

        public double ConvertFromBaseUnit(double value)
        {
            return value / _factor;
        }

        public string GetUnitName()
        {
            return _unit.ToString();
        }

        public string GetMeasurementType()
        {
            return "Weight";
        }
    }
}