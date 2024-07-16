using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OMS.Data.Entites.Accounting;
using OMS.Repositores.DTO;
using OMS.Repositores.Interfaces;
using OMS.Service.ExceptionsHandeling;
using OMS.Service.ServicesJWT;
using OrderMangmentSystem.Helper;

namespace OrderMangmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IConfiguration configuration,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel userLoginDTO)
        {
            var user = await _userManager.FindByEmailAsync(userLoginDTO.Email);
            if (user == null) return Unauthorized(new ApiResponse(401));

            var userPass = await _signInManager.CheckPasswordSignInAsync(user, userLoginDTO.Password, false);

            if (!userPass.Succeeded) return BadRequest(new ApiResponse(401));

            return Ok();

        }
        [AllowAnonymous]
        [HttpPost("Rigester")]
        public async Task<ActionResult<UserDTO>> Rigester(RegisterViewModel registerDTO)
        {

            if (CheckUserExist(registerDTO.Email).Result.Value)
            {
                return BadRequest(new ApiResponse(400, "Email is Already Exist"));
            }
            var user = new AppUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.FullName,
                PhoneNumber = registerDTO.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400));

            var UserResult = new UserDTO()
            {
                Email = registerDTO.Email,
                FullName = registerDTO.FullName,
                Token = await _tokenService.CreateToken(user, _userManager, _configuration)
            };

            return Ok(UserResult);
        }
        [AllowAnonymous]
        [HttpGet("CheckUserExist")]
        public async Task<ActionResult<bool>> CheckUserExist(string email)
        {
            var UserEmail = await _userManager.FineUserWithAddressAsync(User);
            if (UserEmail == null) return false;

            return true;
        }
    }
}
