using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Helpers
{
    public class JwtTokenValidator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool TryValidateToken(string token, out ClaimsPrincipal claimsPrincipal)
        {
            claimsPrincipal = null;

            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Extract the actual token from the "Bearer" header value
            string actualToken = token.Substring("Bearer ".Length).Trim();

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                claimsPrincipal = tokenHandler.ValidateToken(actualToken, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
