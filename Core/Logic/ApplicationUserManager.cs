using Data.DTO;
using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Logic
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> appUser, IOptions<IdentityOptions> options, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer lookupNormalizer, IdentityErrorDescriber identityErrorDescriber, IServiceProvider serviceProvider, ILogger<UserManager<ApplicationUser>> logger, IRepositoryWrapper repository) : base(appUser, options, passwordHasher, userValidators, passwordValidators, lookupNormalizer, identityErrorDescriber, serviceProvider, logger)
        {
        }

        protected static UserDto MapUserInfoData(ApplicationUser appUser)
        {
            return new UserDto
            {
                Id = appUser.Id,
                Address = appUser.Address,
                DateRegistered = appUser.DateRegistered,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Phone = appUser.PhoneNumber,
                Avatar = appUser.Avatar
            };
        }
        protected static IEnumerable<UserDto> ProcessUserInfoData(IQueryable<ApplicationUser> data)
        {
            return data.Select(x => new UserDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.PhoneNumber,
                DateRegistered = x.DateRegistered,
                Address = x.Address,
            });
        }

    }
}
