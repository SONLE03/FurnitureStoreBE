using FurnitureStoreBE.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace FurnitureStoreBE.Config
{
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ApplicationDBContext dbContext;
        public CustomAuthenticationHandler
            (IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder,
            ApplicationDBContext dbContext) : base(options, logger, encoder)
        {
            this.dbContext = dbContext;
            
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var headerToken = Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(headerToken))
                return AuthenticateResult.Fail("Missing Authorization Header");
            try
            {
                headerToken = headerToken.Replace("Bearer ", string.Empty);
                string token = Encoding.UTF8.GetString(Convert.FromBase64String(headerToken));
                var parts = token.Split(".");
                if (parts.Length < 3)
                    return AuthenticateResult.Fail("Invalid Token Format");
                string payload = parts[1];
                Guid userId;
                if (!Guid.TryParse(payload.Trim(), out userId))
                    return AuthenticateResult.Fail("Invalid Token Payload");

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return AuthenticateResult.Fail("User Not Found");
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}
