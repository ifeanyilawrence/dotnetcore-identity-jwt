using Core.Account.Interfaces;
using Core.Logic;
using Data.DTO;
using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Account.Implementation
{
    public class AccountLogic : ApplicationUserManager, IAccountLogic
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IRoleLogic _roleLogic;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IConfiguration _config;
        public AccountLogic(IUserStore<ApplicationUser> appUser, IOptions<IdentityOptions> options, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer lookupNormalizer, IdentityErrorDescriber identityErrorDescriber, IServiceProvider serviceProvider, ILogger<UserManager<ApplicationUser>> logger, IRepositoryWrapper repository, IRoleLogic roleLogic, IApplicationRoleManager roleManager, IConfiguration config) : base(appUser, options, passwordHasher, userValidators, passwordValidators, lookupNormalizer, identityErrorDescriber, serviceProvider, logger, repository)
        {
            _repository = repository;
            _roleLogic = roleLogic;
            _roleManager = roleManager;
            _config = config;
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
        public async Task<UserDto> GetToken(UserDto model)
        {
            ValidateTokenData(model);

            var appUser = await FindByEmailAsync(model.Email);

            if (appUser == null)
                throw new Exception("The username or password is incorrect.");

            bool isPasswordValid = await CheckPasswordAsync(appUser, model.Password);

            if (!isPasswordValid)
                throw new Exception("The username or password is incorrect.");

            ConfirmUserTokenAccess(model, appUser);

            return ProcessLoginToken(appUser);
        }
        private UserDto ProcessLoginToken(ApplicationUser appUser)
        {
            var userClaims = _repository.ApplicationUserClaim.FindByCondition(x => x.UserId == appUser.Id).ToList().Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();

            var userRoles = _repository.ApplicationUserRole.FindByCondition(x => x.UserId == appUser.Id).Select(x => x.RoleId).ToList();
            var roles = _roleManager.Roles.Where(x => userRoles.Contains(x.Id)).Select(x => x.Name).ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, appUser.Email),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim("sub", appUser.Id.ToString()),
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(userClaims);

            var claimsIdentity = new ClaimsIdentity(claims);

            var data = new JwtContainerModel
            {
                SecretKey = _config["Jwt:Key"],
                Subject = claimsIdentity,
                Issuer = _config["Jwt:Issuer"]
            };

            IJwtService authService = new JwtService(data.SecretKey);

            return new UserDto
            {
                Token = authService.GenerateToken(data),
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                Phone = appUser.PhoneNumber,
                Id = appUser.Id,
                DateRegistered = appUser.DateRegistered,
                Address = appUser.Address,
                Avatar = appUser.Avatar,
                Roles = roles.ToArray()
            };
        }

        private void ConfirmUserTokenAccess(UserDto model, ApplicationUser appUser)
        {
            if (!appUser.IsActive)
                throw new Exception("Account is inactive. Contact admin for assistance.");

            if (!appUser.EmailConfirmed)
                throw new Exception("Account email has not been verified. Please verify your email.");
        }
        private void ValidateTokenData(UserDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                throw new Exception("Username and password is required.");


        }
    }
}
