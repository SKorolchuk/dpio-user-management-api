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

        public AccountsController(UserManager<ApplicationUser> userManager, IMapper mapper, IdentityDbContext dbContext)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        // POST api/accounts
        [HttpPost]
        [AllowAnonymous]
		public async Task<IActionResult> Post([FromBody]RegistrationViewModel model)
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
		[Route("forgot")]
		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return Ok();
				}
			}

			return Ok();
		}

		//
		// POST: api/accounts/reset
	    [Route("reset")]
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return Ok();
			}
			var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
			if (result.Succeeded)
			{
				return Ok();
			}
			return View();
		}

	}
}
