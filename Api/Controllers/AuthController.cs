using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Account.Interfaces;
using Data.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAccountLogic _accountLogic;

        public AuthController(IAccountLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]UserDto userData)
        {
            try
            {
                var response = await _accountLogic.RegisterUser(userData);

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}