using BinhTung.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BinhTung.ViewModel
{
    public class ChangePasswordModel
    {
        [Display(Name = "Mật khẩu hiện tại"), Required(ErrorMessage = "Hãy nhập mật khẩu hiện tại"), UIHint("Password")]
        public string OldPassword { get; set; }
        [Display(Name = "Mật khẩu mới"), Required(ErrorMessage = "Hãy nhập mật khẩu mới"),
         StringLength(16, MinimumLength = 4, ErrorMessage = "Mật khẩu từ 4, 16 ký tự"), UIHint("Password")]
        public string Password { get; set; }
        [Display(Name = "Nhập lại mật khẩu"), Compare("Password", ErrorMessage = "Nhập lại mật khẩu không chính xác"),
         UIHint("Password")]
        public string ConfirmPassword { get; set; }
    }
    public class AdminLoginModel
    {
        [Display(Name = "Tên đăng nhập"), Required(ErrorMessage = "Hãy nhập tên đăng nhập")]
        public string Username { get; set; }
        [Display(Name = "Mật khẩu"), Required(ErrorMessage = "Hãy nhập mật khẩu")]
        public string Password { get; set; }
    }
    public class InfoAdminViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
        public IEnumerable<Feedback> Feedbacks { get; set; }
        public IEnumerable<Admin> Admins { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}