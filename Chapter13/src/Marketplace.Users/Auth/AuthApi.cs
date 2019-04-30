using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Users.Auth
{
    [ApiController, Route("api/auth")]
    public class AuthApi : ControllerBase
    {
        readonly AuthService _authService;

        public AuthApi(AuthService authService) => _authService = authService;

        [HttpPost, Route("login")]
        public async Task<IActionResult> Post(Contracts.Login login)
        {
            if (!await _authService.CheckCredentials(
                login.Username, login.Password
            ))
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim("user", login.Password),
                new Claim("name", login.Username),
                new Claim("role", "Member")
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        "user", "role"
                    )
                )
            );

            return Ok();
        }
    }
}