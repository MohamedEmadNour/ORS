using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repositores.DTO
{
    public class LoginViewModel
    {
    
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class RegisterViewModel : LoginViewModel
    {
        [Required]
        [StringLength(128)]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*\d{3})(?=.*[A-Za-z]{3}).{6,}$",
            ErrorMessage = "Password Must Be 6 Char Like 1234*AAaa*1234")]
        public string Password { get; set; }
        public string Roles { get; set; }
    }
    public class UserDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }


}
