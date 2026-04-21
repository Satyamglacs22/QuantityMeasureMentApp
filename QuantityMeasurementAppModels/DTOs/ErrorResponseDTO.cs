namespace QuantityMeasurementAppModels.DTOs
{
    public class ErrorResponseDTO
    {
        public string Timestamp  { get; set; } = string.Empty;
        public int    Status     { get; set; }
        public string Error      { get; set; } = string.Empty;
        public string Message    { get; set; } = string.Empty;
        public string Path       { get; set; } = string.Empty;
    }
}
