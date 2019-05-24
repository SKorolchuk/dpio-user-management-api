using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Deeproxio.AccountApi.Models;
using Deeproxio.AccountApi.Models.Validations;
using Deeproxio.Persistence.Identity.Context;
using Deeproxio.Persistence.Identity.Identity;
using Deeproxio.Persistence.Identity.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Deeproxio.AccountApi.Controllers
{
    [Route("api/[controller]")] 
    public class AccountsController : Controller
    {
        private readonly IdentityDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IdentityDbContext dbContext,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return new OkObjectResult(new {
                User.Identity
            });
        }

        // POST api/accounts/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.UserName, _jwtOptions,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                });
            return new OkObjectResult(jwt);
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

            return new OkObjectResult(new
            {
                Status = "Account created"
            });
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

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}
