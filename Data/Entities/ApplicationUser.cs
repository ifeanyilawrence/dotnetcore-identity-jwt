using Microsoft.AspNetCore.Identity;
using System;

namespace Data.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
        public DateTime DateRegistered { get; set; }
    }
}
