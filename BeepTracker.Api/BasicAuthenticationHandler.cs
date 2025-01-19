using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace BeepTracker.Api
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
            //_userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? username = null;
            try
            {
                var authHeaderReq = Request.Headers["Authorization"];
                if(string.IsNullOrEmpty( authHeaderReq) || authHeaderReq.Count == 0 || authHeaderReq == default(StringValues))
                {
                    throw new Exception("Authorisation was not passed in the header");
                }
                var authHeader = AuthenticationHeaderValue.Parse(authHeaderReq);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter ?? "")).Split(':');
                username = credentials.FirstOrDefault();
                var password = credentials.LastOrDefault();

                // todo validate user against our database
                //if (!_userService.ValidateCredentials(username, password))
                //    throw new ArgumentException("Invalid credentials");
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }

            var claims = new[] {
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
