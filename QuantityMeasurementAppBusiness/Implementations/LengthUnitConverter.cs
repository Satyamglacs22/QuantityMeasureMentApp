using QuantityMeasurementAppModels.Enums;
using QuantityMeasurementAppBusiness.Interfaces;

namespace QuantityMeasurementAppBusiness.Implementations
{
    public sealed class LengthUnitConverter : IMeasurable
    {
        private readonly double _factor;
        private readonly LengthUnit _unit;

        private LengthUnitConverter(double factor, LengthUnit unit)
        {
            _factor = factor;
            _unit = unit;
        }

        public static readonly LengthUnitConverter Feet        = new LengthUnitConverter(1.0,         LengthUnit.Feet);
        public static readonly LengthUnitConverter Inches      = new LengthUnitConverter(1.0 / 12.0,  LengthUnit.Inches);
        public static readonly LengthUnitConverter Yards       = new LengthUnitConverter(3.0,         LengthUnit.Yards);
        public static readonly LengthUnitConverter Centimeters = new LengthUnitConverter(0.032808,    LengthUnit.Centimeters);

        public static LengthUnitConverter FromEnum(LengthUnit unit)
        {
            if (unit == LengthUnit.Feet)        return Feet;
            if (unit == LengthUnit.Inches)      return Inches;
            if (unit == LengthUnit.Yards)       return Yards;
            if (unit == LengthUnit.Centimeters) return Centimeters;
            throw new ArgumentException("Unknown LengthUnit: " + unit);
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
            return "Length";
        }
    }
}