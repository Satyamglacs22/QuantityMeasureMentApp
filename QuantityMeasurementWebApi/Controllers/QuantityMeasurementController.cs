using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementAppModels.DTOs;
using QuantityMeasurementAppServices.Interfaces;

namespace QuantityMeasurementWebApi.Controllers
{
    /// <summary>
    /// REST API for quantity measurement operations
    /// </summary>
    [ApiController]
    [Route("api/v1/quantities")]
    public class QuantityMeasurementController : ControllerBase
    {
        private readonly IQuantityWebService service;

        public QuantityMeasurementController(IQuantityWebService service)
        {
            this.service = service;
        }

        // ==================== POST Endpoints ====================

        /// <summary>
        /// Compare two quantities for equality
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [HttpPost("compare")]
        [ProducesResponseType(typeof(QuantityMeasurementResponseDTO), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public IActionResult Compare([FromBody] QuantityInputRequest request)
        {
            QuantityMeasurementResponseDTO result = service.Compare(request);
            return Ok(result);
        }

        /// <summary>
        /// Convert a quantity to a different unit
        /// </summary>
        /// <remarks>
        /// Example request:
        ///
        ///     
        ///
        /// </remarks>
        [HttpPost("convert")]
        [ProducesResponseType(typeof(QuantityMeasurementResponseDTO), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public IActionResult Convert([FromBody] ConvertRequest request)
        {
            QuantityMeasurementResponseDTO result = service.Convert(request);
            return Ok(result);
        }

        /// <summary>
        /// Add two quantities together
        /// </summary>
        /// <remarks>
        ///
        ///
        /// </remarks>
        [HttpPost("add")]
        [ProducesResponseType(typeof(QuantityMeasurementResponseDTO), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public IActionResult Add([FromBody] ArithmeticRequest request)
        {
            QuantityMeasurementResponseDTO result = service.Add(request);
            return Ok(result);
        }

        /// <summary>
        /// Subtract one quantity from another
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [HttpPost("subtract")]
        [ProducesResponseType(typeof(QuantityMeasurementResponseDTO), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public IActionResult Subtract([FromBody] ArithmeticRequest request)
        {
            QuantityMeasurementResponseDTO result = service.Subtract(request);
            return Ok(result);
        }

        /// <summary>
        /// Divide one quantity by another (returns dimensionless ratio)
        /// </summary>
        /// <remarks>
        /// Example request:
        ///
        ///     POST /api/v1/quantities/divide
        ///     {
        ///         "thisQuantityDTO": { "value": 2.0, "unitName": "Feet", "measurementType": "Length" },
        ///         "thatQuantityDTO": { "value": 1.0, "unitName": "Feet", "measurementType": "Length" }
        ///     }
        ///
        /// </remarks>
        [HttpPost("divide")]
        [ProducesResponseType(typeof(QuantityMeasurementResponseDTO), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public IActionResult Divide([FromBody] QuantityInputRequest request)
        {
            QuantityMeasurementResponseDTO result = service.Divide(request);
            return Ok(result);
        }

        // ==================== GET Endpoints ====================

        /// <summary>
        /// Get operation history filtered by operation type
        /// </summary>
        /// <param name="operation">Operation name: Add, Subtract, Divide, Compare, Convert</param>
        [HttpGet("history/operation/{operation}")]
        [ProducesResponseType(typeof(List<QuantityMeasurementResponseDTO>), 200)]
        public IActionResult GetHistoryByOperation(string operation)
        {
            List<QuantityMeasurementResponseDTO> result = service.GetHistoryByOperation(operation);
            return Ok(result);
        }

        /// <summary>
        /// Get history filtered by measurement type
        /// </summary>
        /// <param name="type">Measurement type: Length, Weight, Volume, Temperature</param>
        [HttpGet("history/type/{type}")]
        [ProducesResponseType(typeof(List<QuantityMeasurementResponseDTO>), 200)]
        public IActionResult GetHistoryByType(string type)
        {
            List<QuantityMeasurementResponseDTO> result = service.GetHistoryByType(type);
            return Ok(result);
        }

        /// <summary>
        /// Get all records that resulted in an error
        /// </summary>
        [HttpGet("history/errored")]
        [ProducesResponseType(typeof(List<QuantityMeasurementResponseDTO>), 200)]
        public IActionResult GetErrorHistory()
        {
            List<QuantityMeasurementResponseDTO> result = service.GetErrorHistory();
            return Ok(result);
        }

        /// <summary>
        /// Get count of successful operations by operation type
        /// </summary>
        /// <param name="operation">Operation name: Add, Subtract, Divide, Compare, Convert</param>
        [HttpGet("count/{operation}")]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult GetOperationCount(string operation)
        {
            int count = service.GetOperationCount(operation);
            return Ok(count);
        }
    }
}
