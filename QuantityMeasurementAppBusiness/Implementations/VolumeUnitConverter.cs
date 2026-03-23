using QuantityMeasurementAppModels.Enums;
using QuantityMeasurementAppBusiness.Interfaces;

namespace QuantityMeasurementAppBusiness.Implementations
{
    public sealed class VolumeUnitConverter : IMeasurable
    {
        private readonly double _factor;
        private readonly VolumeUnit _unit;

        private VolumeUnitConverter(double factor, VolumeUnit unit)
        {
            _factor = factor;
            _unit = unit;
        }

        public static readonly VolumeUnitConverter Litre      = new VolumeUnitConverter(1.0,     VolumeUnit.Litre);
        public static readonly VolumeUnitConverter Millilitre = new VolumeUnitConverter(0.001,   VolumeUnit.Millilitre);
        public static readonly VolumeUnitConverter Gallon     = new VolumeUnitConverter(3.78541, VolumeUnit.Gallon);

        public static VolumeUnitConverter FromEnum(VolumeUnit unit)
        {
            if (unit == VolumeUnit.Litre)      return Litre;
            if (unit == VolumeUnit.Millilitre) return Millilitre;
            if (unit == VolumeUnit.Gallon)     return Gallon;
            throw new ArgumentException("Unknown VolumeUnit: " + unit);
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
            return "Volume";
        }
    }
}