using Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private ApplicationDbContext _applicationDbContext;
        private ApplicationUserRepository _applicationUser;
        private ApplicationRoleRepository _applicationRole;
        private ApplicationUserRoleRepository _applicationUserRole;
        private ApplicationUserClaimRepository _applicationUserClaim;

        public RepositoryWrapper(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IApplicationUserRepository ApplicationUser { 
            get {
                if (_applicationUser == null)
                    _applicationUser = new ApplicationUserRepository(_applicationDbContext);

                return _applicationUser;
            }
        }

        public IApplicationRoleRepository ApplicationRole { 
            get {
                if (_applicationRole == null)
                    _applicationRole = new ApplicationRoleRepository(_applicationDbContext);

                return _applicationRole;
            }
        }

        public IApplicationUserRoleRepository ApplicationUserRole { 
            get {
                if (_applicationUserRole == null)
                    _applicationUserRole = new ApplicationUserRoleRepository(_applicationDbContext);

                return _applicationUserRole;
            }
        }

        public IApplicationUserClaimRepository ApplicationUserClaim
        {
            get
            {
                if (_applicationUserClaim == null)
                    _applicationUserClaim = new ApplicationUserClaimRepository(_applicationDbContext);

                return _applicationUserClaim;
            }
        }

        public async Task SaveAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
