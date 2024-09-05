using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FurnitureStoreBE.Exceptions
{
    public class DefaultExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<DefaultExceptionHandler> _logger;
        public DefaultExceptionHandler(ILogger<DefaultExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);
            if (exception is BusinessException businessException)
            {
                httpContext.Response.StatusCode = businessException.StatusCode;
                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Type = businessException.GetType().Name,
                    Status = businessException.StatusCode,
                    Detail = businessException.Message,
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                });

                return true;
            }
            return false;
        }
    }
}
