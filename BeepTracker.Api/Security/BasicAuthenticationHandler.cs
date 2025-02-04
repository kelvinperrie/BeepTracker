using Azure.Core;
using BeepTracker.Api.Services;
using BeepTracker.Common.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace BeepTracker.Api.Security
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserService _userService;

        public BasicAuthenticationHandler(UserService userService,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
            _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? username = null;
            User foundUser;
            try
            {
                var authHeaderReq = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeaderReq) || authHeaderReq.Count == 0 || authHeaderReq == default(StringValues))
                {
                    throw new Exception("Authorisation was not passed in the header");
                }
                var authHeader = AuthenticationHeaderValue.Parse(authHeaderReq);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter ?? "")).Split(':');
                username = credentials.FirstOrDefault();
                var password = credentials.LastOrDefault();

                // validate user against our database
                foundUser = _userService.GetUserByUsernameAndPassword(username, password);

                if(foundUser == null)
                {
                    throw new Exception($"Attempting to connect with user ({username}) and password but could not find match in database");
                }

                if(foundUser.Active == false)
                {
                    throw new Exception($"Attempted connect with user {username}, but the user is not active");
                }

            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }

            var claims = new[] {
                new Claim(ClaimTypes.Name, username),
                new Claim("User.Id", foundUser.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
