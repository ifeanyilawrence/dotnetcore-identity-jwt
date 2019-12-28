using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interfaces
{
    public interface IApplicationUserClaimRepository : IRepositoryBase<ApplicationUserClaim>
    {
        Task<IEnumerable<ApplicationUserClaim>> GetAllUserClaimsAsync();
        Task<ApplicationUserClaim> GetUserClaimByIdAsync(Guid userClaimId);
        void CreateUserClaim(ApplicationUserClaim userClaim);
        void UpdateUserClaim(ApplicationUserClaim userClaim);
        void DeleteUserClaim(ApplicationUserClaim userClaim);
    }
}
