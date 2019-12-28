using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
    public class ApplicationUserClaimRepository : RepositoryBase<ApplicationUserClaim>, IApplicationUserClaimRepository
    {
        public ApplicationUserClaimRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        { 

        }

        public async Task<IEnumerable<ApplicationUserClaim>> GetAllUserClaimsAsync()
        {
            return await FindAll().ToListAsync();
        }

        public async Task<ApplicationUserClaim> GetUserClaimByIdAsync(Guid userClaimId)
        {
            return await FindByCondition(x => x.Id.Equals(userClaimId)).FirstOrDefaultAsync();
        }
        public void CreateUserClaim(ApplicationUserClaim userClaim)
        {
            Create(userClaim);
        }

        public void UpdateUserClaim(ApplicationUserClaim userClaim)
        {
            Update(userClaim);
        }

        public void DeleteUserClaim(ApplicationUserClaim userClaim)
        {
            Delete(userClaim);
        }
    }
}
