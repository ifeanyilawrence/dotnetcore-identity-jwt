using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Core.Account.Interfaces
{
    public interface IJwtContainerModel
    {
        string SecretKey { get; set; }
        string SecurityAlgorithm { get; set; }
        int ExpireMinutes { get; set; }
        ClaimsIdentity Subject { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
    }
}
