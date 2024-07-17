using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OMS.Data.Entites;
using OMS.Data.Entites.Accounting;
using OMS.Data.Entites.Const;
using OMS.Repositores.DTO;
using OMS.Repositores.Interfaces;
using OMS.Repositores.Repositories;
using OMS.Service.ExceptionsHandeling;
using OMS.Service.ServicesJWT;
using OrderMangmentSystem.Helper;
using System.Security.Claims;
using Xunit.Sdk;

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
        private readonly IUnitOfWork _UnitOfWork;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IConfiguration configuration,
            IMapper mapper,
            IUnitOfWork UnitOfWork
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _mapper = mapper;
            _UnitOfWork = UnitOfWork;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel userLoginDTO)
        {
            var user = await _userManager.FindByEmailAsync(userLoginDTO.Email);
            if (user == null) return Unauthorized(new ApiResponse(401));

            var userPass = await _signInManager.CheckPasswordSignInAsync(user, userLoginDTO.Password, false);

            if (!userPass.Succeeded) return BadRequest(new ApiResponse(401));

            var userResult = new UserDTO()
            {
                Email = user.Email,
                FullName = user.UserName,
                Token = await _tokenService.CreateToken(user, _userManager, _configuration)

            };
            return Ok(userResult);

        }

        [AllowAnonymous]
        [HttpPost("Rigester")]
        public async Task<ActionResult<UserDTO>> Register(RegisterViewModel registerDTO)
        {
            if (CheckUserExist(registerDTO.Email).Result.Value)
            {
                return BadRequest(new ApiResponse(400, "Email is already in use"));
            }

            var user = new AppUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.FullName,
                PhoneNumber = registerDTO.PhoneNumber
            };

            IdentityResult result;

            if (registerDTO.CreatorEmail != null)
            {
                result = await RegisterAdminUser(registerDTO.CreatorEmail, user, registerDTO.Password);
            }
            else
            {
                result = await RegisterCustomerUser(registerDTO.Email, user, registerDTO.Password);
            }

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Failed to create user"));
            }

            var userResult = new UserDTO
            {
                Email = registerDTO.Email,
                FullName = registerDTO.FullName,
                Token = await _tokenService.CreateToken(user, _userManager, _configuration)
            };

            return Ok(userResult);
        }



        private async Task<ActionResult<bool>> CheckUserExist(string email)
        {
            var UserEmail = await _userManager.FineUserWithAddressAsync(User);
            if (UserEmail == null) return false;

            return true;
        }


        private async Task<IdentityResult> RegisterAdminUser(string creatorEmail, AppUser user, string password)
        {
            var admin = await _userManager.FindByEmailAsync(creatorEmail);
            if (admin == null) BadRequest(new ApiResponse(400, "Please contact support"));

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return result;

            var roleResult = await _userManager.AddToRoleAsync(user, RolesConstants.Admin);
            if (!roleResult.Succeeded) BadRequest(new ApiResponse(400, "Failed to assign role to user"));

            return IdentityResult.Success;
        }

        private async Task<IdentityResult> RegisterCustomerUser(string email, AppUser user, string password)
        {
            var customer = await _UnitOfWork.repositories<Customer, int>().FindCustomerByEmail(email);
            if (customer == null) BadRequest(new ApiResponse(400, "Please contact support"));

            user.UserName = customer.Name;
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return result;

            var roleResult = await _userManager.AddToRoleAsync(user, RolesConstants.User);
            if (!roleResult.Succeeded) BadRequest(new ApiResponse(400, "Failed to assign role to user"));

            return IdentityResult.Success;
        }
    }
}
