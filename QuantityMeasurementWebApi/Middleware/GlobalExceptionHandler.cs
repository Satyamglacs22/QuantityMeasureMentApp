using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuantityMeasurementAppBusiness.Exceptions;
using QuantityMeasurementAppModels.DTOs;

namespace QuantityMeasurementWebApi.Middleware
{
    // Centralized exception handler — registered as an MVC action filter
    public class GlobalExceptionHandler : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Decide HTTP status code based on exception type
            int    status = GetStatusCode(context.Exception);
            string error  = GetErrorLabel(status);

            // Build a structured error response
            ErrorResponseDTO response = new ErrorResponseDTO
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                Status    = status,
                Error     = error,
                Message   = context.Exception.Message,
                Path      = context.HttpContext.Request.Path.ToString()
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = status
            };

            // Mark exception as handled so ASP.NET Core does not re-throw
            context.ExceptionHandled = true;
        }

        // Returns 400 for known domain exceptions, 500 for everything else
        private int GetStatusCode(Exception exception)
        {
            if (exception is QuantityMeasurementException) return 400;
            if (exception is ArgumentException)            return 400;
            if (exception is NotSupportedException)        return 400;
            return 500;
        }

        private string GetErrorLabel(int status)
        {
            return status == 400 ? "Bad Request" : "Internal Server Error";
        }
    }
}
