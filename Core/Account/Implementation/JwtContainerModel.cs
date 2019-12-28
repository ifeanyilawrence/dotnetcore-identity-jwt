using Core.Account.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Core.Account.Implementation
{
    public class JwtContainerModel : IJwtContainerModel
    {
        public string SecretKey { get; set; }

        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public int ExpireMinutes { get; set; } = 120;
        public ClaimsIdentity Subject { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; } = "All";
    }
}
