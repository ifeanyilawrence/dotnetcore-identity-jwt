using Core.Account.Interfaces;
using Core.Logic;
using Data.DTO;
using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Account.Implementation
{
    public class RoleLogic : ApplicationUserManager, IRoleLogic
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IApplicationRoleManager _roleManager;

        public RoleLogic(IUserStore<ApplicationUser> appUser, IOptions<IdentityOptions> options, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer lookupNormalizer, IdentityErrorDescriber identityErrorDescriber, IServiceProvider serviceProvider, ILogger<UserManager<ApplicationUser>> logger, IRepositoryWrapper repository, IApplicationRoleManager roleManager) : base(appUser, options, passwordHasher, userValidators, passwordValidators, lookupNormalizer, identityErrorDescriber, serviceProvider, logger, repository)
        {
            _repository = repository;
            _roleManager = roleManager;
        }

        public async Task AddUserClaims(Dictionary<string, string> associatedClaims, ApplicationUser appUser)
        {
            if (associatedClaims == null || !associatedClaims.Any()) return;
            foreach (var claim in associatedClaims)
            {
                var userClaim = await _repository.ApplicationUserClaim
                                        .FindByCondition(x => appUser.Id == x.UserId && x.ClaimType == claim.Key && x.ClaimValue == claim.Value).SingleOrDefaultAsync();

                if (userClaim != null) continue;

                userClaim = new ApplicationUserClaim
                {
                    ClaimType = claim.Key,
                    ClaimValue = claim.Value,
                    UserId = appUser.Id
                };
                _repository.ApplicationUserClaim.CreateUserClaim(userClaim);
                await _repository.SaveAsync();
            }
        }
        public async Task AddToRoleAndClaims(UserDto callerData, RoleUpdateDto model)
        {
            var roles = model.Roles.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();

            if (roles.Select(x => x.ToLower()).Contains("superadmin"))
                throw new Exception("Cannot Create SuperUser at this time");

            var appUser = await FindByIdAsync(model.UserId.ToString());

            if (appUser == null)
                throw new Exception("User not found");

            await AddUserToRoles(roles, appUser);

            await AddUserClaims(model.AssociatedClaims, appUser);
        }
        public async Task<ApplicationUser> RemoveRoleAndClaims(RoleUpdateDto model)
        {
            var roles = model.Roles.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            var appUser = await FindByIdAsync(model.UserId.ToString());
            if (appUser == null)
                throw new Exception("User not found");
            foreach (var role in roles)
            {
                var appRole = await GetApplicationRole(role);
                var appUserRole = await _repository.ApplicationUserRole.FindByCondition(x => x.RoleId == appRole.Id).SingleOrDefaultAsync();
                if (appUserRole != null)
                {
                    _repository.ApplicationUserRole.Delete(appUserRole);
                }
            }

            await RemoveUserClaims(model.AssociatedClaims, appUser);

            return appUser;
        }
        private async Task RemoveUserClaims(Dictionary<string, string> associatedClaims, ApplicationUser appUser)
        {
            if (associatedClaims.Any())
            {
                foreach (var claim in associatedClaims)
                {
                    var userClaim = await _repository.ApplicationUserClaim.FindByCondition(x => appUser.Id == x.UserId && x.ClaimType == claim.Key && x.ClaimValue == claim.Value).SingleOrDefaultAsync();

                    _repository.ApplicationUserClaim.Delete(userClaim);
                }
            }
        }
        public async Task AddUserToRoles(string[] roles, ApplicationUser user)
        {
            foreach (var role in roles)
            {
                await AddToRole(user.Id, role);
            }
        }
        public async Task AddToRole(Guid userId, string role)
        {
            var appUser = await _repository.ApplicationUser.GetUserByIdAsync(userId);
            if (appUser == null)
                throw new Exception("User not found.");
            await AddToRole(role, appUser);
        }
        public async Task AddToRole(string role, ApplicationUser appUser)
        {
            var appRole = await GetApplicationRole(role);
            var appUserRole = await _repository.ApplicationUserRole.FindByCondition(x => x.UserId == appUser.Id && x.RoleId == appRole.Id).SingleOrDefaultAsync();
            if (appUserRole == null)
            {
                appUserRole = new ApplicationUserRole
                {
                    RoleId = appRole.Id,
                    UserId = appUser.Id
                };

                _repository.ApplicationUserRole.Create(appUserRole);

                await _repository.SaveAsync();
            }
        }
        protected async Task<ApplicationRole> GetApplicationRole(string role)
        {
            var appRole = await _roleManager.FindByNameAsync(role);
            if (appRole == null)
            {
                appRole = new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = role
                };
                await _roleManager.CreateAsync(appRole);
            }
            return appRole;
        }
    }
}
