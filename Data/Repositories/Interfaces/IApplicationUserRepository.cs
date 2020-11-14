using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interfaces
{
    public interface IApplicationUserRepository : IRepositoryBase<ApplicationUser>
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(Guid userId);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> GetUserWithDetailsAsync(Guid userId);
        void CreateUser(ApplicationUser user);
        void UpdateUser(ApplicationUser user);
        void DeleteUser(ApplicationUser user);
    }
    public interface IOrderRepository : IRepository<ApplicationUser>
    {
        IQueryable<ApplicationUser> Queryable(int countryId);
        IQueryable<ApplicationUser> Filter(OrderFilter filter = null);
        IQueryable<ApplicationUser> GetPaged(int page, int count, out int totalCount, OrderFilter filter = null);
        IQueryable<ApplicationUser> GetPaged(int page, int count, OrderFilter filter = null);
    }
}
