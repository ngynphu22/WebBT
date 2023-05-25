using BinhTung.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BinhTung.ViewModel
{
    public class ListContactViewModel
    {
        public PagedList.IPagedList<Contact> Contacts { get; set; }
        public string Name { get; set; }
    }
    public class ListFeedbackViewModel
    {
        public PagedList.IPagedList<Feedback> Feedbacks { get; set; }
        public string Name { get; set; }
    }
}