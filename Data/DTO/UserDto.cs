using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DTO
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid Id { get; set; }
        public string Address { get; set; }
        public DateTime DateRegistered { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public bool IsEmailVerified { get; set; }
        public string[] Roles { get; set; }
        public Dictionary<string, string> AssociatedClaims { get; set; }
        public string Token { get; set; }
    }
}
