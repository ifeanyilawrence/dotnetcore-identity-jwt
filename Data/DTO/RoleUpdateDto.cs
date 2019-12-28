using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DTO
{
    public class RoleUpdateDto
    {
        private string[] _roles;

        public Guid UserId { get; set; }

        public string[] Roles
        {
            get { return _roles ?? new string[] { }; }
            set { _roles = value; }
        }
        private Dictionary<string, string> _associatedClaims;

        public Dictionary<string, string> AssociatedClaims
        {
            get { return _associatedClaims ?? new Dictionary<string, string>(); }
            set { _associatedClaims = value; }
        }
    }
}
