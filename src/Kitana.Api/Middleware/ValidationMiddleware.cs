
using Amazon.Runtime;
using CsvHelper.Configuration.Attributes;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kitana.Api.Middleware
{
    /// <summary>
    /// The validation middleware takes in the jwt token and validates it based on the exp date and also fetches all the parameters
    /// </summary>
    public class ValidationMiddleware
    {
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationMiddleware"/> class with the specified <see cref="IHttpContextAccessor"/>.
        /// </summary>
        /// <param name="contextAccessor">The <see cref="IHttpContextAccessor"/> providing access to the current HTTP context.</param>
        /// <remarks>
        /// This constructor allows the <c>ValidationMiddleware</c> to be instantiated with a reference to a specific
        /// <c>IHt,jktpContextAccessor</c>, enabling access to the current HTTP context for validation purposes.
        /// </remarks>
        public ValidationMiddleware(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// The function takes in token and fetches all the parameters inside and puts them inside param model to be used anywhere in the programm
        /// </summary>
        public bool IsValid(string token, string validations)
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var tokens = handler.ReadJwtToken(token);
                var validParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
                    ValidAudience = Environment.GetEnvironmentVariable("Jwt_Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt_Key")))
                };
                handler.ValidateToken(token, validParams, out var validToken);

                var claims = tokens.Claims;
                var role = claims.FirstOrDefault(c => c.Type == "role")?.Value;
                var userId = claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                var superEmail = claims.FirstOrDefault(c => c.Type == "email")?.Value;
                _contextAccessor.HttpContext.Session.SetString("Email", superEmail);
                _contextAccessor.HttpContext.Session.SetString("Role", role);
                _contextAccessor.HttpContext.Session.SetString("UserId", userId);
                bool containsRole = validations.Split(',').Any(r => r.Trim() == role);

                return containsRole;
            }
            return false;
        }
    }
}
