using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BinhTung.Models
{
    public class Contact
    {
        public int Id { get; set; }
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Hãy nhập họ và tên"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string Fullname { get; set; }
        [Display(Name = "Địa chỉ"), Required(ErrorMessage = "Hãy nhập địa chỉ"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string Address { get; set; }
        [Display(Name = "Số điện thoại"), Required(ErrorMessage = "Hãy nhập số điện thoại"), Phone(ErrorMessage = "Số điện thoại không hợp lệ"), UIHint("TextBox")]
        public string Mobile { get; set; }
        [Display(Name = "Email"), Required(ErrorMessage = "Hãy nhập địa chỉ email"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Nội dung"), DataType(DataType.MultilineText), StringLength(4000)]
        public string Body { get; set; }
        public DateTime CreateDate { get; set; }

        public Contact()
        {
            CreateDate = DateTime.Now;
        }
    }
}