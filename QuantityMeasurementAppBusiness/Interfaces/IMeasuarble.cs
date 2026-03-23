namespace QuantityMeasurementAppBusiness.Interfaces
{
    public interface IMeasurable
    {
        double ConvertToBaseUnit(double value);

        double ConvertFromBaseUnit(double baseValue);

        string GetUnitName();

        string GetMeasurementType();

        // default behaviour (C# style)
        bool SupportsArithmetic()
        {
            return true;
        }

        void ValidateOperationSupport(string operation)
        {
            // default: do nothing
        }
    }
}