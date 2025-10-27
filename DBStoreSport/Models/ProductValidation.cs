using System.ComponentModel.DataAnnotations;

namespace DBStoreSport.Models
{
    [MetadataType(typeof(ProductValidation))]
    public partial class Product
    {
    }

    public class ProductValidation
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên sản phẩm không được vượt quá 50 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string NamePro { get; set; }

        [StringLength(50, ErrorMessage = "Mô tả không được vượt quá 50 ký tự")]
        [Display(Name = "Mô tả")]
        public string DecriptionPro { get; set; }

        [Display(Name = "Danh mục")]
        public int? CateID { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        [Display(Name = "Giá")]
        public decimal? Price { get; set; }

        [StringLength(50, ErrorMessage = "Tên file hình ảnh không được vượt quá 50 ký tự")]
        [Display(Name = "Hình ảnh")]
        public string ImagePro { get; set; }
    }
} 