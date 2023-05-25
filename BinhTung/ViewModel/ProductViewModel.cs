using BinhTung.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BinhTung.ViewModel
{
    public class ListProductViewModel
    {
        public PagedList.IPagedList<Product> Products { get; set; }
        public SelectList SelectCategories { get; set; }
        public SelectList ChildCategoryList { get; set; }
        public int? ParentId { get; set; }
        public int? CatId { get; set; }
        public string Name { get; set; }
        public string Sort { get; set; }

        public ListProductViewModel()
        {
            ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");
        }
    }
    public class InsertProductViewModel
    {
        public Product Product { get; set; }
        [Display(Name = "Danh mục sản phẩm con"), Required(ErrorMessage = "Hãy chọn danh mục sản phẩm")]
        public int ParentId { get; set; }
        [Display(Name = "Danh mục sản phẩm cha")]
        public int? CategoryId { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public SelectList SelectCategories { get; set; }
        public SelectList ChildCategoryList { get; set; }
        public SelectList ProductCategoryList { get; set; }
        public InsertProductViewModel()
        {
            ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");
        }
    }
}