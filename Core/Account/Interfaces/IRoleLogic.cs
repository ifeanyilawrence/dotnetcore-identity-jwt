using Data.DTO;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Account.Interfaces
{
    public interface IRoleLogic
    {
        Task AddToRoleAndClaims(UserDto userDto, RoleUpdateDto model);
        Task<ApplicationUser> RemoveRoleAndClaims(UserDto userDto, RoleUpdateDto model);

        Task AddUserToRoles(string[] roles, ApplicationUser user);

        Task AddUserClaims(Dictionary<string, string> associatedClaims, ApplicationUser appUser);
        Task AddToRole(string role, ApplicationUser appUser);
    }
}
