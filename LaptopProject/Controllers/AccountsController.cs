using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models.DTOs;

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
                await _signInManager.SignInAsync(user, false);
                return Created();
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
                    await _signInManager.SignInAsync(appUser,
                        log.RememberMe);
                    return NoContent();
                }

            }

            ModelState.AddModelError(string.Empty, "Your account or Password is not correct");
            return BadRequest(ModelState);
        }

        [HttpGet("LogOut")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }




    }



}
