using QuantityMeasurementAppBusiness.Interfaces;
using QuantityMeasurementAppModels.Enums;

namespace QuantityMeasurementAppBusiness.Implementations
{
    public class VolumeMeasurementImpl : IMeasurable
    {
        private readonly VolumeUnit unit;

        public VolumeMeasurementImpl(VolumeUnit unitType)
        {
            unit = unitType;
        }

        public double GetConversionFactor()
        {
            return unit switch
            {
                VolumeUnit.Litre      => 1.0,
                VolumeUnit.Millilitre => 0.001,
                VolumeUnit.Gallon     => 3.78541,
                _                     => throw new ArgumentException("Invalid Volume Unit")
            };
        }

        public double ConvertToBaseUnit(double value)   => value * GetConversionFactor();
        public double ConvertFromBaseUnit(double value) => value / GetConversionFactor();
        public string GetUnitName()                     => unit.ToString();
        public bool   SupportsArithmetic()              => true;
        public void   ValidateOperationSupport(string operation) { }
        public string GetMeasurementType()              => "Volume";

        public IMeasurable GetUnitByName(string unitName)
        {
            VolumeUnit parsed = (VolumeUnit)Enum.Parse(typeof(VolumeUnit), unitName, true);
            return new VolumeMeasurementImpl(parsed);
        }
    }
}
