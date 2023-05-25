using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BinhTung.Models
{
    public class ArticleCategory
    {
        public int Id { get; set; }
        [Display(Name = "Tên danh mục"), Required(ErrorMessage = "Hãy nhập tên danh mục"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string CategoryName { get; set; }
        [Display(Name = "Trích dẫn ngắn"),
        StringLength(700, ErrorMessage = "Tối đa 700 ký tự"), DataType(DataType.MultilineText)]
        public string Description { get; set; }
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
        [Display(Name = "Hiển menu")]
        public bool ShowMenu { get; set; }

        [Display(Name = "Thẻ tiêu đề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string TitleMeta { get; set; }
        [Display(Name = "Thẻ mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string DescriptionMeta { get; set; }
        [Display(Name = "Ảnh bìa"), StringLength(500), UIHint("ImageArticleCat")]
        public string Image { get; set; }
        public TypePost TypePost { get; set; }
        public virtual ICollection<Article> Articles { get; set; }

        public ArticleCategory()
        {
            ShowHome = false;
            CategoryActive = true;
        }
    }
    public enum TypePost
    {
        [Display(Name = "Tin tức")]
        News,
        [Display(Name = "Chính sách")]
        Support
    }
}