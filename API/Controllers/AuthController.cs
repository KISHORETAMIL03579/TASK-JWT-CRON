using Application.DTO;
using INFRASTRUCTURE.AUTH;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = FakeUsers.Users.FirstOrDefault(x => x.Username == dto.Username && x.Password == dto.Password);

            if (user == null) return Unauthorized();

            var token = new JwtService().GenerateToken(user, _config);

            return Ok(new { token });
        }
    }
}
