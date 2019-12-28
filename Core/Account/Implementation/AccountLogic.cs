using Core.Account.Interfaces;
using Core.Logic;
using Data.DTO;
using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Account.Implementation
{
    public class AccountLogic : ApplicationUserManager, IAccountLogic
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IRoleLogic _roleLogic;
        public AccountLogic(IUserStore<ApplicationUser> appUser, IOptions<IdentityOptions> options, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer lookupNormalizer, IdentityErrorDescriber identityErrorDescriber, IServiceProvider serviceProvider, ILogger<UserManager<ApplicationUser>> logger, IRepositoryWrapper repository, IRoleLogic roleLogic) : base(appUser, options, passwordHasher, userValidators, passwordValidators, lookupNormalizer, identityErrorDescriber, serviceProvider, logger, repository)
        {
            _repository = repository;
            _roleLogic = roleLogic;
        }

        public async Task<UserDto> RegisterUser(UserDto data)
        {
            try
            {
                ValidateRequestData(data);
                await CheckUserExistBeforeCreate(data);
                var appUser = await CreateApplicationUser(data);
                return await CompleteRegisterUser(data, appUser);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private async Task<UserDto> CompleteRegisterUser(UserDto data, ApplicationUser appUser)
        {
            try
            {
                if (!appUser.EmailConfirmed)
                {
                    var token = await GenerateEmailConfirmationTokenAsync(appUser);
                    var registrationToken = WebUtility.UrlEncode(token);
                    //_emailHelper.SendRegistrationMail(countryConfig, data, registrationToken);
                }

                await CompleteCreateApplicationUser(data, appUser);

                await _repository.SaveAsync();

                return MapUserInfoData(appUser);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private async Task CompleteCreateApplicationUser(UserDto data, ApplicationUser appUser)
        {
            await _roleLogic.AddUserToRoles(data.Roles, appUser);

            await _roleLogic.AddUserClaims(data.AssociatedClaims, appUser);
        }
        private void ValidateRequestData(UserDto data)
        {
            if (string.IsNullOrWhiteSpace(data.Password))
                throw new Exception("Password is required.");
            if (string.IsNullOrWhiteSpace(data.Email))
                throw new Exception("Email is required.");
            if (!IsValidEmail(data.Email))
                throw new Exception("Invalid email.");
        }
        private bool IsValidEmail(string strIn)
        {
            // Return true if strIn is invalid email format.
            try
            {
                return Regex.IsMatch(strIn,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        private async Task CheckUserExistBeforeCreate(UserDto data)
        {
            var appUser = await FindByEmailAsync(data.Email);
            if (appUser != null)
            {
                throw new Exception("Account exists. Please login to continue.");
            }
        }
        private async Task<ApplicationUser> CreateApplicationUser(UserDto data)
        {
            var appUser = await FindByEmailAsync(data.Email);
            if (appUser == null)
            {
                appUser = new ApplicationUser
                {
                    UserName = data.Email,
                    Email = data.Email,
                    PhoneNumber = data.Phone,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    IsActive = true,
                    DateRegistered = DateTime.Now,
                    Address = data.Address,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false
                };

                IdentityResult result;

                if (!string.IsNullOrWhiteSpace(data.Password))
                    result = await CreateAsync(appUser, data.Password);
                else
                    result = await CreateAsync(appUser);

                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);
            }

            data.IsEmailVerified = appUser.EmailConfirmed;

            return appUser;
        }
    }
}
