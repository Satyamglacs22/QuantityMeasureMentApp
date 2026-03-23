using System;

namespace QuantityMeasurementAppBusiness.Exceptions
{
    // Purpose:
    // Wraps raw SqlException into a friendlier application-level exception.
    // Upper layers (Service, Controller) catch DatabaseException
    // instead of SqlException so they stay decoupled from the database.

    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message)
        {
        }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}