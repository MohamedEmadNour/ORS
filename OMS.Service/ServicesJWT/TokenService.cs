using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OMS.Data.Entites.Accounting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.ServicesJWT
{
    public class TokenService : ITokenService
    {


        public async Task<string> CreateToken(AppUser user , UserManager<AppUser> _userManager , IConfiguration _Configuration)
        {
            var Auth = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var Role in userRoles)
            {

                Auth.Add(new Claim(ClaimTypes.Role,Role));

            }

            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                issuer: _Configuration["JWT:ValidationIssuer"],
                audience: _Configuration["JWT:Validationaudience"],
                expires: DateTime.Now.AddDays(double.Parse(_Configuration["JWT:Validationexpires"])),
                claims : Auth,
                signingCredentials: new SigningCredentials(AuthKey , SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
