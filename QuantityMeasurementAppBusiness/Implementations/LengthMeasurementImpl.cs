using QuantityMeasurementAppBusiness.Interfaces;
using QuantityMeasurementAppModels.Enums;

namespace QuantityMeasurementAppBusiness.Implementations
{
    public class LengthMeasurementImpl : IMeasurable
    {
        private readonly LengthUnit unit;

        public LengthMeasurementImpl(LengthUnit unitType)
        {
            unit = unitType;
        }

        public double GetConversionFactor()
        {
            return unit switch
            {
                LengthUnit.Feet       => 1.0,
                LengthUnit.Inch       => 1.0 / 12.0,
                LengthUnit.Yard       => 3.0,
                LengthUnit.Centimeter => 0.0328084,
                _                     => throw new ArgumentException("Invalid Length Unit")
            };
        }

        public double ConvertToBaseUnit(double value)   => value * GetConversionFactor();
        public double ConvertFromBaseUnit(double value) => value / GetConversionFactor();
        public string GetUnitName()                     => unit.ToString();
        public bool   SupportsArithmetic()              => true;
        public void   ValidateOperationSupport(string operation) { }
        public string GetMeasurementType()              => "Length";

        public IMeasurable GetUnitByName(string unitName)
        {
            LengthUnit parsed = (LengthUnit)Enum.Parse(typeof(LengthUnit), unitName, true);
            return new LengthMeasurementImpl(parsed);
        }
    }
}
