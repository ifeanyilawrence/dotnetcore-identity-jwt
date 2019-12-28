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
    public class ApplicationUserRepository : RepositoryBase<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        { 

        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await FindAll().OrderBy(x => x.LastName).ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(Guid userId)
        {
            return await FindByCondition(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await FindByCondition(x => x.Email.Equals(email)).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> GetUserWithDetailsAsync(Guid userId)
        {
            return await FindByCondition(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
        }

        public void CreateUser(ApplicationUser user)
        {
            Create(user);
        }

        public void UpdateUser(ApplicationUser user)
        {
            Update(user);
        }

        public void DeleteUser(ApplicationUser user)
        {
            Delete(user);
        }
    }
}
