using System;

namespace QuantityMeasurementApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            QuantityMeasurementApp app = QuantityMeasurementApp.GetInstance();

            try
            {
                app.Start();
                app.ReportAllMeasurements();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Program] Unhandled error: " + ex.Message);
            }
            finally
            {
                app.CloseResources();
            }
        }
    }
}
