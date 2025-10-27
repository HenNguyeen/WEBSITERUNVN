using System;
using System.ComponentModel.DataAnnotations;

namespace DBStoreSport.Models
{
    public class VoucherValidation
    {
        [Required(ErrorMessage = "Mã voucher là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã voucher không được vượt quá 50 ký tự")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Mã voucher chỉ được chứa chữ cái và số")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Giá trị giảm là bắt buộc")]
        [Range(1, 100, ErrorMessage = "Giá trị giảm phải từ 1 đến 100")]
        public int DiscountValue { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày hết hạn")]
        public DateTime ExpiryDate { get; set; }

        public bool? IsUsed { get; set; }
    }
} 