using System.ComponentModel.DataAnnotations;

namespace DBStoreSport.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu ít nhất 8 ký tự")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
