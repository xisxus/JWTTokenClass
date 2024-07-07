using ApiWIthJWTAuth.Contacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sharedlaibary.DTOs;

namespace ApiWIthJWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController(IUserAccount userAccount) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            var response=await userAccount.CreateAccount(userDTO);
            return Ok(response);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await userAccount.LoginAccount(loginDTO);
            return Ok(response);
        }
    }
}
