using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DBStoreSport.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không đúng.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không đúng.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}