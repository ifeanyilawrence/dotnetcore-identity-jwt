using System;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities
{
    public class ApplicationUserLogin: IdentityUserLogin<Guid> { }
}