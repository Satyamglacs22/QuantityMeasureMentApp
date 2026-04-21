using QuantityMeasurementAppBusiness.Interfaces;
using QuantityMeasurementAppModels.Enums;

namespace QuantityMeasurementAppBusiness.Implementations
{
    public class TemperatureMeasurementImpl : IMeasurable
    {
        private readonly TemperatureUnit unit;

        public TemperatureMeasurementImpl(TemperatureUnit unitType)
        {
            unit = unitType;
        }

        public double GetConversionFactor() => 1.0;

        public double ConvertToBaseUnit(double value)
        {
            return unit switch
            {
                TemperatureUnit.Celsius    => value,
                TemperatureUnit.Fahrenheit => (value - 32) * 5.0 / 9.0,
                TemperatureUnit.Kelvin     => value - 273.15,
                _                          => throw new ArgumentException("Invalid Temperature Unit")
            };
        }

        public double ConvertFromBaseUnit(double baseValue)
        {
            return unit switch
            {
                TemperatureUnit.Celsius    => baseValue,
                TemperatureUnit.Fahrenheit => (baseValue * 9.0 / 5.0) + 32,
                TemperatureUnit.Kelvin     => baseValue + 273.15,
                _                          => throw new ArgumentException("Invalid Temperature Unit")
            };
        }

        public string GetUnitName()        => unit.ToString();
        public bool   SupportsArithmetic() => false;
        public string GetMeasurementType() => "Temperature";

        public void ValidateOperationSupport(string operation)
        {
            throw new NotSupportedException("Temperature does not support " + operation + " operation");
        }

        public IMeasurable GetUnitByName(string unitName)
        {
            TemperatureUnit parsed = (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), unitName, true);
            return new TemperatureMeasurementImpl(parsed);
        }
    }
}
