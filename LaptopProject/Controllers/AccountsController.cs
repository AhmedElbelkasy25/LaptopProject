using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LaptopProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegiesterDTO userLog)
        {



            var user = new ApplicationUser
            {
                UserName = userLog.UserName,
                Email = userLog.Email,

            };
            var usrTest = await _userManager.FindByEmailAsync(user.Email);
            if (usrTest != null)
            {
                ModelStateDictionary keyValuePairs = new ModelStateDictionary();
                keyValuePairs.AddModelError("Email", "this account is already exist");
                ModelState.AddModelError("Email", "this account is already exist...");
                return BadRequest(ModelState);
            }

            var result = await _userManager.CreateAsync(user, userLog.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                //await _signInManager.SignInAsync(user, false);
                // ✅ Create JWT token
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, user.Id)
    };
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Guendouzie29Guendouzie29Guendouzie29"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: "https://localhost:7213",
                    audience: "https://localhost:4200",
                    claims: claims,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: creds
                );
                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                ModelStateDictionary keyValuePairs = new ModelStateDictionary();
                keyValuePairs.AddModelError("Password", "Password must be has a Capital and Small letters and numbers");

                return BadRequest(keyValuePairs);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO log)
        {


            var appUserWithEmail = await _userManager.FindByEmailAsync(log.Account);
            var appUserWithUserName = await _userManager.FindByNameAsync(log.Account);
            var appUser = appUserWithEmail ?? appUserWithUserName;
            if (appUser != null)
            {
                if (await _userManager.IsLockedOutAsync(appUser))
                {
                    ModelState.AddModelError(string.Empty, "This account has been blocked.");
                    return BadRequest(ModelState);
                }
                //var result = await _userManager.CheckPasswordAsync(appUserWithEmail ?? appUserWithUserName , log.Password);
                var result = await _userManager.CheckPasswordAsync(appUser, log.Password);
                if (result)
                {
                    //await _signInManager.SignInAsync(appUser,
                    //    log.RememberMe);

                    var userRole = await _userManager.GetRolesAsync(appUser);
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name ,appUser.UserName ) ,
                        new Claim(ClaimTypes.NameIdentifier ,appUser.Id )
                    };
                    foreach (var role in userRole)
                    {
                        claims.Add(new Claim(ClaimTypes.Role,role));
                    }

                    SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Guendouzie29Guendouzie29Guendouzie29"));

                    SigningCredentials signingCredentials = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);
                    JwtSecurityToken token = new(
                        issuer: "https://localhost:7213",
                        audience: "https://localhost:4200",
                        claims : claims,
                        expires:DateTime.Now.AddHours(3),
                        signingCredentials: signingCredentials
                        );

                    return Ok(new
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }

            }

            ModelState.AddModelError(string.Empty, "Your account or Password is not correct");
            return BadRequest(ModelState);
        }

        [HttpGet("LogOut")]
        public async Task<IActionResult> Logout()
        {
            //await _signInManager.SignOutAsync();
            return NoContent();
        }




    }



}
