using AirportControlTower.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Text;

namespace AirportControlTower.Api.MIddleware
{
    public class AdminAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AdminCredentials _credentials;

        public AdminAuthMiddleware(
            RequestDelegate next,
            IOptions<AdminCredentials> credentials)
        {
            _next = next;
            _credentials = credentials.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (path != null && path.StartsWith("/api/admin"))
            {
                if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Missing Authorization header");
                    return;
                }

                if (!authHeader.ToString().StartsWith("Basic "))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid Authorization scheme");
                    return;
                }

                var encoded = authHeader.ToString().Substring("Basic ".Length).Trim();

                string decoded;
                try
                {
                    var bytes = Convert.FromBase64String(encoded);
                    decoded = Encoding.UTF8.GetString(bytes);
                }
                catch
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid Base64 encoding");
                    return;
                }

                var parts = decoded.Split(':');
                if (parts.Length != 2)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid Authorization format");
                    return;
                }

                var username = parts[0];
                var password = parts[1];

                if (username != _credentials.Username || password != _credentials.Password)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid credentials");
                    return;
                }
            }

            await _next(context);
        }
    }
}
