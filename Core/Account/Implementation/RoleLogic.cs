using Core.Account.Interfaces;
using Core.Logic;
using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void AddUserClaims(int countryId, Dictionary<string, string> associatedClaims,
            ApplicationUser appUser)
        {
            if (!associatedClaims.Any()) return;
            foreach (var claim in associatedClaims)
            {
                var userClaim = _claimRepository.Table
                    .SingleOrDefault(x => x.CountryId == countryId &&
                                          appUser.Id == x.UserId && x.ClaimType == claim.Key &&
                                          x.ClaimValue == claim.Value);
                if (userClaim != null) continue;

                userClaim = new ApplicationUserClaim
                {
                    ClaimType = claim.Key,
                    ClaimValue = claim.Value,
                    CountryId = countryId,
                    UserId = appUser.Id
                };
                _claimRepository.Insert(userClaim);
                _claimRepository.SaveChanges();
            }
        }
        public async Task AddToRoleAndClaims(CallerUserData callerData, RoleUpdateData model)
        {
            var roles = model.Roles.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
            if (roles.Select(x => x.ToLower()).Contains("superadmin"))
                throw new SureGroupException("Cannot Create SuperUser at this time");
            var appUser = await FindByIdAsync(model.UserId);
            if (appUser == null)
                throw new SureGroupException("User not found");
            await AddUserToRoles(callerData.CountryId, roles, appUser);
            AddUserClaims(callerData.CountryId, model.AssociatedClaims, appUser);
            var description = $"Added User to Role(s) and Claim(s) - Email:{appUser.Email}, Role: {string.Join(",", model.Roles)} Claims:{string.Join(",", model.AssociatedClaims.Select(x => x.Value))}";
            _auditLogic.LogAction(callerData, "Added User to Role(s) and Claim(s)", description, OperationTypeEnum.Other);
        }
        public async Task<ApplicationUser> RemoveRoleAndClaims(CallerUserData callerData, RoleUpdateData model)
        {
            var roles = model.Roles.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            var appUser = await FindByIdAsync(model.UserId);
            if (appUser == null)
                throw new SureGroupException("User not found");
            foreach (var role in roles)
            {
                var appRole = await GetApplicationRole(role);
                var appUserRole = appUser.Roles.SingleOrDefault(x => x.CountryId == callerData.CountryId && x.RoleId == appRole.Id);
                if (appUserRole != null)
                {
                    _roleRepository.Delete(appUserRole);
                }
            }
            RemoveUserClaims(callerData, model.AssociatedClaims, appUser);
            var hasRoles = _roleRepository.Table.Any(x => x.UserId == appUser.Id && x.CountryId == callerData.CountryId);
            var hasClaims = _claimRepository.Table.Any(x => x.CountryId == callerData.CountryId && x.UserId == appUser.Id);
            if (!hasClaims && !hasRoles)
            {
                var access = _accessRepository.Table.SingleOrDefault(x =>
                    x.UserId == appUser.Id && x.CountryId == callerData.CountryId);
                if (access != null)
                    _accessRepository.Delete(access);
            }
            var description = $"Removed User from Role(s) and Claim(s) - Email:{appUser.Email}, Role: {string.Join(",", model.Roles)} Claims:{string.Join(",", model.AssociatedClaims.Select(x => x.Value))}";
            _auditLogic.LogAction(callerData, "Removed User from Role(s) and Claim(s)", description, OperationTypeEnum.Other);
            return appUser;
        }
        private void RemoveUserClaims(CallerUserData callerData, Dictionary<string, string> associatedClaims,
            ApplicationUser appUser)
        {
            if (associatedClaims.Any())
            {
                foreach (var claim in associatedClaims)
                {
                    var userClaim = _claimRepository.Table
                        .SingleOrDefault(x => x.CountryId == callerData.CountryId &&
                                              appUser.Id == x.UserId && x.ClaimType == claim.Key &&
                                              x.ClaimValue == claim.Value);
                    _claimRepository.Delete(userClaim);
                }
            }
        }
        public async Task AddUserToRoles(int countryId, string[] roles, ApplicationUser user)
        {
            foreach (var role in roles)
            {
                await AddToRole(countryId, user.Id, role);
            }
        }
        public async Task AddToRole(int countryId, Guid userId, string role)
        {
            var appUser = _repository.Table.Include(x => x.Roles).SingleOrDefault(x => x.Id == userId);
            if (appUser == null)
                throw new SureGroupException("User not found.");
            await AddToRole(countryId, role, appUser);
        }
        public async Task AddToRole(int countryId, string role, ApplicationUser appUser)
        {
            var appRole = await GetApplicationRole(role);
            var appUserRole = appUser.Roles.SingleOrDefault(x => x.UserId == appUser.Id && x.CountryId == countryId && x.RoleId == appRole.Id);
            if (appUserRole == null)
            {
                appUser.Roles.Add(new ApplicationUserRole
                {
                    RoleId = appRole.Id,
                    UserId = appUser.Id,
                    CountryId = countryId,
                });
                _repository.SaveChanges();
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
