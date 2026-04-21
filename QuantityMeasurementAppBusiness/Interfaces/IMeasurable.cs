namespace QuantityMeasurementAppBusiness.Interfaces
{
    public interface IMeasurable
    {
        double GetConversionFactor();
        double ConvertToBaseUnit(double value);
        double ConvertFromBaseUnit(double baseValue);
        string GetUnitName();
        bool SupportsArithmetic();
        void ValidateOperationSupport(string operation);
        string GetMeasurementType();
        IMeasurable GetUnitByName(string unitName);
    }
}
