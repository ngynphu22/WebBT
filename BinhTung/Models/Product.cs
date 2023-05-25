using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BinhTung.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Display(Name = "Tên sản phẩm", Description = "Tên sản phẩm dài tối đa 150 ký tự"),
         Required(ErrorMessage = "Hãy nhập Tên sản phẩm"), StringLength(150, ErrorMessage = "Tối đa 150 ký tự"),
         UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Thông tin sản phẩm"), Required(ErrorMessage = "Thông tin sản phẩm"),
         StringLength(600, ErrorMessage = "Tối đa 600 ký tự"), DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Mô tả chi tiết"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Danh sách ảnh"), UIHint("UploadMultiFile")]
        public string ListImage { get; set; }
        [Display(Name = "Số lượng"), Required(ErrorMessage = "Hãy nhập số lượng"), RegularExpression(@"\d+", ErrorMessage = "Nhập số nguyên dương"), UIHint("NumberBox")]
        public int Quantity { get; set; }
        [Display(Name = "Giá gốc"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? Price { get; set; }
        [Display(Name = "Giảm giá"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? SaleOff { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Ngày đăng")]
        public DateTime CreateDate { get; set; }
        public int View { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Danh mục sản phẩm"), Required(ErrorMessage = "Hãy chọn danh mục sản phẩm")]
        public int ProductCategoryId { get; set; }
        [Display(Name = "Hiện nổi bật")]
        public bool Hot { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Hiện trang chủ")]
        public bool Home { get; set; }
        [StringLength(300)]
        public string Url { get; set; }
        [Display(Name = "Thẻ tiêu đề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string TitleMeta { get; set; }
        [Display(Name = "Thẻ mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string DescriptionMeta { get; set; }
        [Display(Name = "Loại sản phẩm")]
        public virtual ProductCategory ProductCategory { get; set; }

        public Product()
        {
            CreateDate = DateTime.Now;
            Active = true;
            View = 1;
        }
    }
}