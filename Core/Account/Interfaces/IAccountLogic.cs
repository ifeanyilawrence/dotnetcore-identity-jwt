using Data.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Account.Interfaces
{
    public interface IAccountLogic
    {
        Task<UserDto> RegisterUser(UserDto userData);
        Task<UserDto> GetToken(UserDto userData);
    }
}
