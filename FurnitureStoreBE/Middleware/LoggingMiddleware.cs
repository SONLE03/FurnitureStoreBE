using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var remoteAddress = context.Connection.RemoteIpAddress?.ToString();
        var method = context.Request.Method;
        var path = context.Request.Path;

        Log.Information($"Incoming request from {remoteAddress}: {method} {path}");

        await _next(context); // Call the next middleware

        Log.Information($"Response sent: {context.Response.StatusCode}");
    }
}
