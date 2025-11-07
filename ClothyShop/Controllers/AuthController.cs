using ClothyShop.Business.Services;
using ClothyShop.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClothyShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            var success = await _userService.RegisterAsync(dto.Email, dto.Password, dto.Role);
            if (!success)
                return BadRequest("User already exists.");

            return Redirect("/");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto login)  // <-- FromForm here
        {
            var user = await _userService.AuthenticateAsync(login.Email, login.Password);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "ClothyShopAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("ClothyShopAuth", principal);

            // After successful login, redirect to home page or dashboard
            return Redirect("/");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("ClothyShopAuth");
            return Redirect("/");
        }
    }
}
