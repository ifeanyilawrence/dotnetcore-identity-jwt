using Core.Account.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Core.Account.Implementation
{
    public class JwtService : IJwtService
    {
        public string SecretKey { get; set; }
        public JwtService(string secretKey)
        {
            SecretKey = secretKey;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            return jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        }
        public string GenerateToken(IJwtContainerModel model)
        {
            if (model?.Subject == null || !model.Subject.Claims.Any())
                throw new Exception("Arguments to create token are not valid.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = model.Issuer,
                Audience = model.Audience,
                Subject = model.Subject,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(model.ExpireMinutes)),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            string token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return token;
        }


        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new Exception("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return tokenValid.Claims;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(SecretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = GetSymmetricSecurityKey()
            };
        }
        public string ReissueToken(string token)
        {
            ClaimsPrincipal claimsPrincipal = ValidateToken(token);
            return GenerateToken(new JwtContainerModel()
            {
                Subject = new ClaimsIdentity(claimsPrincipal.Claims)
            });
        }
    }
}
