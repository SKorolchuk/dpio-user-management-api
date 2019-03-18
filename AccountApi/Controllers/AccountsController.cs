using System.Threading.Tasks;
using AutoMapper;
using Deeproxio.AccountApi.Models;
using Deeproxio.AccountApi.Models.Validations;
using Deeproxio.Persistence.Identity.Context;
using Deeproxio.Persistence.Identity.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Deeproxio.AccountApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly IdentityDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IdentityDbContext dbContext)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return new OkObjectResult(new
            {
                User.Identity
            });
        }

        // POST api/accounts/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<ApplicationUser>(model);

            var result = await _userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            await _dbContext.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }

        //
        // POST: api/accounts/forgot
        [HttpPost("forgot")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return NotFound(model);
                }
                return new OkObjectResult(user);
            }

            return BadRequest(model);
        }

        //
        // POST: api/accounts/reset
        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return NotFound(model);
            }

            var result = await _userManager.ResetPasswordAsync(user,
                await _userManager.GeneratePasswordResetTokenAsync(user), model.Password);
            if (result.Succeeded)
            {
                return new OkObjectResult(result);
            }

            return BadRequest(model);
        }
    }
}
