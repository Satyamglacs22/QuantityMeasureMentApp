using QuantityMeasurementAppBusiness.Interfaces;
using QuantityMeasurementAppModels.Enums;

namespace QuantityMeasurementAppBusiness.Implementations
{
    public class WeightMeasurementImpl : IMeasurable
    {
        private readonly WeightUnit unit;

        public WeightMeasurementImpl(WeightUnit unitType)
        {
            unit = unitType;
        }

        public double GetConversionFactor()
        {
            return unit switch
            {
                WeightUnit.Kilogram => 1.0,
                WeightUnit.Gram     => 0.001,
                WeightUnit.Pound    => 0.453592,
                _                   => throw new ArgumentException("Invalid Weight Unit")
            };
        }

        public double ConvertToBaseUnit(double value)   => value * GetConversionFactor();
        public double ConvertFromBaseUnit(double value) => value / GetConversionFactor();
        public string GetUnitName()                     => unit.ToString();
        public bool   SupportsArithmetic()              => true;
        public void   ValidateOperationSupport(string operation) { }
        public string GetMeasurementType()              => "Weight";

        public IMeasurable GetUnitByName(string unitName)
        {
            WeightUnit parsed = (WeightUnit)Enum.Parse(typeof(WeightUnit), unitName, true);
            return new WeightMeasurementImpl(parsed);
        }
    }
}
