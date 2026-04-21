using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementAppModels.DTOs
{
    public class QuantityInputRequest
    {
        [Required(ErrorMessage = "ThisQuantityDTO is required")]
        public QuantityDTO ThisQuantityDTO { get; set; } = new QuantityDTO();

        [Required(ErrorMessage = "ThatQuantityDTO is required")]
        public QuantityDTO ThatQuantityDTO { get; set; } = new QuantityDTO();
    }
}
