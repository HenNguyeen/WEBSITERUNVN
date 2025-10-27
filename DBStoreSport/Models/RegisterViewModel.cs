using System.ComponentModel.DataAnnotations;

namespace DBStoreSport.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập bắt buộc")]
        [MinLength(8, ErrorMessage = "Tên đăng nhập phải ít nhất 8 ký tự,")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
            ErrorMessage = "Tên đăng nhập phải chứa cả chữ và số (ít nhất 8 ký tự)")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải ít nhất 8 ký tự")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ in hoa và 1 chữ số")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string NameCus { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneCus { get; set; }
    }
}
