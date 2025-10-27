using System.ComponentModel.DataAnnotations;

namespace DBStoreSport.Models
{
    public class EditProfileViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
