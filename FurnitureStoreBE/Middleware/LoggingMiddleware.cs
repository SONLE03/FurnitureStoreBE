using FurnitureStoreBE.Common;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

public class HeaderCheckMiddleware
{
    private readonly RequestDelegate _next;

    public HeaderCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
        {
            await _next(context);
            return;
        }

        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", string.Empty);
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    UserSession.SetUserId(userId); // Store userId in the static class
                    Console.WriteLine($"User ID from JWT: {userId}");
                }
                else
                {
                    Console.WriteLine("User ID claim not found in token.");
                }
            }
            else
            {
                Console.WriteLine("Invalid JWT token.");
            }
        }
        else
        {
            Console.WriteLine("Authorization header not found.");
        }
        await _next(context);
    }
}

// Middleware để ghi log
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
