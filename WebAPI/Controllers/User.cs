using Domain.Dto;
using Domain.Interface;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User : ControllerBase
    {
        private readonly IUser user;

        public User(IUser user)
        {
            this.user = user;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> LogUserIn(LoginDto loginDto)
        {
            var result = await user.LoginUserAsync(loginDto);

            return Ok(result);
        }
        [HttpPost("Register")]
        public async Task<ActionResult<RegistrationResponse>> RegisterUser(RegisterUserDto registerUserDto)
        {
            var result = await user.RegisterUserAsync(registerUserDto);

            return Ok(result);
        }
    }
}
