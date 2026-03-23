using QuantityMeasurementAppModels.Enums;
using QuantityMeasurementAppBusiness.Interfaces;

namespace QuantityMeasurementAppBusiness.Implementations
{
    public sealed class TemperatureUnitConverter : IMeasurable
    {
        private readonly TemperatureUnit _unit;

        private TemperatureUnitConverter(TemperatureUnit unit)
        {
            _unit = unit;
        }

        public static readonly TemperatureUnitConverter CELSIUS    = new TemperatureUnitConverter(TemperatureUnit.CELSIUS);
        public static readonly TemperatureUnitConverter FAHRENHEIT = new TemperatureUnitConverter(TemperatureUnit.FAHRENHEIT);
        public static readonly TemperatureUnitConverter KELVIN     = new TemperatureUnitConverter(TemperatureUnit.KELVIN);

        public static TemperatureUnitConverter FromEnum(TemperatureUnit unit)
        {
            if (unit == TemperatureUnit.CELSIUS)    return CELSIUS;
            if (unit == TemperatureUnit.FAHRENHEIT) return FAHRENHEIT;
            if (unit == TemperatureUnit.KELVIN)     return KELVIN;
            throw new ArgumentException("Unknown TemperatureUnit: " + unit);
        }

        public double ConvertToBaseUnit(double value)
        {
            if (_unit == TemperatureUnit.CELSIUS)    return value;
            if (_unit == TemperatureUnit.FAHRENHEIT) return (value - 32.0) * 5.0 / 9.0;
            if (_unit == TemperatureUnit.KELVIN)     return value - 273.15;
            throw new ArgumentException("Unknown TemperatureUnit: " + _unit);
        }

        public double ConvertFromBaseUnit(double baseValue)
        {
            if (_unit == TemperatureUnit.CELSIUS)    return baseValue;
            if (_unit == TemperatureUnit.FAHRENHEIT) return (baseValue * 9.0 / 5.0) + 32.0;
            if (_unit == TemperatureUnit.KELVIN)     return baseValue + 273.15;
            throw new ArgumentException("Unknown TemperatureUnit: " + _unit);
        }

        public string GetUnitName()
        {
            return _unit.ToString();
        }

        public string GetMeasurementType()
        {
            return "Temperature";
        }

        public bool SupportsArithmetic()
        {
            return false;
        }

        public void ValidateOperationSupport(string operation)
        {
            throw new NotSupportedException("Temperature does not support arithmetic operations.");
        }
    }
}