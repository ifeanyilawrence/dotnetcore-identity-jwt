using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Core.Account.Interfaces
{
    public interface IJwtService
    {
        string SecretKey { get; set; }
        ClaimsPrincipal ValidateToken(string token);
        string GenerateToken(IJwtContainerModel model);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
