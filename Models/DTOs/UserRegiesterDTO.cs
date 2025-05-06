using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class UserRegiesterDTO
    {
        
        [Required]
        [Length(5, 50)]
        public string UserName { get; set; }

        [Required]
        [Length(6, 50)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Length(6, 50)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "ConfirmPassword")]
        public string conPassword { get; set; }
    }


    public class LoginUserDTO
    {
        
        [Required]
        [Display(Name = "UserName Or Email")]
        public string Account { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
