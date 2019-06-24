using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace MiddlewareExample
{
    public class CustomAuthSchemaDefaults
    {
        public const string AuthenticationScheme = "Custom";
        public const string ADMIN = "Admin";

        public static AuthenticationTicket CreateTicket()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "test")
            };

            claims.Add(new Claim(ClaimTypes.Role, ADMIN));

            var claimsIdentity = new ClaimsIdentity(claims, AuthenticationScheme);

            return new AuthenticationTicket(
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { AllowRefresh = false },
                AuthenticationScheme);
        }
    }

    public class CustomAuthSchemaOptions : AuthenticationSchemeOptions { }

    public class CustomAuthSchemaHandler : AuthenticationHandler<CustomAuthSchemaOptions>
    {
        /// <inheritdoc />
        public CustomAuthSchemaHandler(IOptionsMonitor<CustomAuthSchemaOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <inheritdoc />
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var handler = this;
            string header = handler.Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(header))
            {
                return AuthenticateResult.NoResult();
            }

            if (header.ToUpperInvariant().StartsWith("BEARER"))
            {
                return AuthenticateResult.Success(CustomAuthSchemaDefaults.CreateTicket());
            }

            return AuthenticateResult.NoResult();

        }

        /// <inheritdoc />
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var handler = this;
            handler.Response.StatusCode = StatusCodes.Status401Unauthorized;
            handler.Response.Headers[HeaderNames.WWWAuthenticate] =
                CustomAuthSchemaDefaults.AuthenticationScheme;
        }
    }
}
