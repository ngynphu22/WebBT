using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BinhTung.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        [Display(Name = "Tên danh mục"), Required(ErrorMessage = "Hãy nhập tên danh mục"), StringLength(80, ErrorMessage = "Tối đa 80 ký tự"), UIHint("TextBox")]
        public string CategoryName { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int CategorySort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool CategoryActive { get; set; }
        [Display(Name = "Danh mục cha")]
        public int? ParentId { get; set; }
        [Display(Name = "Hiển thị trang chủ")]
        public bool ShowHome { get; set; }
        [Display(Name = "Hiển thị chân trang")]
        public bool ShowFooter { get; set; }
        [Display(Name = "Đặc biệt")]
        public bool Special { get; set; }
        [Display(Name = "Thẻ tiêu đề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string TitleMeta { get; set; }
        [Display(Name = "Thẻ mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string DescriptionMeta { get; set; }
        [Display(Name = "Giới thiệu ngắn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }
        [Display(Name = "Ảnh đại diện"), StringLength(500), UIHint("ImageProductCat")]
        public string Image { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public ProductCategory()
        {
            ShowHome = false;
            CategoryActive = true;
        }
    }
}