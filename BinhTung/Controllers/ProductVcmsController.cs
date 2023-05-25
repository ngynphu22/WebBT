using BinhTung.DAL;
using BinhTung.Models;
using BinhTung.ViewModel;
using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BinhTung.Controllers
{
    [Authorize]
    public class ProductVcmsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private IEnumerable<ProductCategory> ProductCategories => _unitOfWork.ProCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private SelectList ParentProductCategoryList => new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");

        #region ProductCategory
        [ChildActionOnly]
        public ActionResult ListCategory()
        {
            var allcats = _unitOfWork.ProCategoryRepository.Get(orderBy: q => q.OrderBy(a => a.CategorySort));
            return PartialView(allcats);
        }
        public ActionResult ProductCategory(string result = "")
        {
            ViewBag.ArticleCat = result;
            ViewBag.RootCats =
                new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(new ProductCategory());
        }
        [HttpPost]
        public ActionResult ProductCategory(ProductCategory category, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        isPost = false;
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            category.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            //var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            //HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }
                if (isPost)
                {
                    category.Url = HtmlHelpers.ConvertToUnSign(null, category.Url ?? category.CategoryName);
                    _unitOfWork.ProCategoryRepository.Insert(category);
                    _unitOfWork.Save();
                    return RedirectToAction("ProductCategory", new { result = "success" });
                }
            }
            ViewBag.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(category);
        }
        public ActionResult UpdateCategory(int catId = 0)
        {
            var category = _unitOfWork.ProCategoryRepository.GetById(catId);
            if (category == null)
            {
                return RedirectToAction("ProductCategory");
            }
            ViewBag.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(category);
        }
        [HttpPost]
        public ActionResult UpdateCategory(ProductCategory category, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        isPost = false;
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            if (System.IO.File.Exists(Server.MapPath("/images/productCategory/" + category.Image)))
                            {
                                System.IO.File.Delete(Server.MapPath("/images/productCategory/" + category.Image));
                            }
                            category.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            //var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            //HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }
                else
                {
                    category.Image = fc["CurrentFile"];
                }
                if (isPost)
                {
                    category.Url = HtmlHelpers.ConvertToUnSign(null, category.Url ?? category.CategoryName);
                    _unitOfWork.ProCategoryRepository.Update(category);
                    _unitOfWork.Save();
                    return RedirectToAction("ProductCategory", new { result = "update" });
                }
            }
            ViewBag.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(category);
        }
        [HttpPost]
        public bool DeleteCategory(int catId = 0)
        {
            var category = _unitOfWork.ProCategoryRepository.GetById(catId);
            if (category == null)
            {
                return false;
            }
            _unitOfWork.ProCategoryRepository.Delete(category);
            _unitOfWork.Save();
            return true;
        }
        public bool UpdateProductCat(int sort = 1, bool active = false, bool special = false, bool home = false, bool footer = false, int productCatId = 0)
        {
            var productCat = _unitOfWork.ProCategoryRepository.GetById(productCatId);
            if (productCat == null)
            {
                return false;
            }
            productCat.CategorySort = sort;
            productCat.CategoryActive = active;
            productCat.Special = special;
            productCat.ShowHome = home;
            productCat.ShowFooter = footer;

            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Product
        public ActionResult ListProduct(int? page, string name, int? parentId, int catId = 0, string sort = "date-desc", string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var products = _unitOfWork.ProductRepository.GetQuery().AsNoTracking();
            if (catId > 0)
            {
                products = products.Where(l => l.ProductCategoryId == catId);
            }
            else if (parentId != null)
            {
                products = products.Where(a => a.ProductCategoryId == parentId || a.ProductCategory.ParentId == parentId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(l => l.Name.ToLower().Contains(name.ToLower()));
            }

            switch (sort)
            {
                case "sort-asc":
                    products = products.OrderBy(a => a.Sort);
                    break;
                case "sort-desc":
                    products = products.OrderByDescending(a => a.Sort);
                    break;
                case "date-asc":
                    products = products.OrderBy(a => a.CreateDate);
                    break;
                default:
                    products = products.OrderByDescending(a => a.CreateDate);
                    break;
            }
            var model = new ListProductViewModel
            {
                SelectCategories = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                Products = products.ToPagedList(pageNumber, pageSize),
                CatId = catId,
                ParentId = parentId,
                Name = name,
                Sort = sort
            };
            if (parentId.HasValue)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == parentId), "Id", "Categoryname");
            }
            return View(model);
        }
        public ActionResult Product(int? catId, int parentId = 0)
        {
            var model = new InsertProductViewModel
            {
                ProductCategoryList = ParentProductCategoryList,
                ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName"),
                Product = new Product { Sort = 1 },
                ParentId = parentId,
                CategoryId = catId,
            };
            if (parentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == parentId), "Id", "CategoryName");
            }
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Product(InsertProductViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.Product.ListImage = fc["Pictures"];
                model.Product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                model.Product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                _unitOfWork.ProductRepository.Insert(model.Product);
                _unitOfWork.Save();
                return RedirectToAction("ListProduct", new { result = "success" });
            }
            model.ProductCategoryList = ParentProductCategoryList;
            model.ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");

            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        public ActionResult UpdateProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }
            var model = new InsertProductViewModel
            {
                Product = product,
                Categories = ProductCategories,
                ParentId = product.ProductCategory.ParentId ?? product.ProductCategoryId,
                ProductCategoryList = ParentProductCategoryList,
                CategoryId = product.ProductCategoryId,
            };
            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateProduct(InsertProductViewModel model, FormCollection fc)
        {
            var product = _unitOfWork.ProductRepository.GetById(model.Product.Id);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }
            if (ModelState.IsValid)
            {
                product.ListImage = fc["Pictures"] == "" ? null : fc["Pictures"];
                product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                product.Name = model.Product.Name;
                product.Description = model.Product.Description;
                product.Body = model.Product.Body;
                product.Active = model.Product.Active;
                product.Home = model.Product.Home;
                product.Hot = model.Product.Hot;
                product.Quantity = model.Product.Quantity;
                product.TitleMeta = model.Product.TitleMeta;
                product.DescriptionMeta = model.Product.DescriptionMeta;
                product.Sort = model.Product.Sort;

                _unitOfWork.Save();
                return RedirectToAction("ListProduct", new { result = "update" });
            }
            model.ProductCategoryList = ParentProductCategoryList;
            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            _unitOfWork.ProductRepository.Delete(product);
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool CloneProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            _unitOfWork.ProductRepository.Insert(product);
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool QuickUpdate(int? quantity, bool? status, bool active, bool home, int sort = 0, int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            if (status != null)
            {
                product.Active = Convert.ToBoolean(status);
            }
            if (quantity.HasValue)
            {
                product.Quantity = Convert.ToInt32(quantity);
            }
            if (sort >= 0)
            {
                product.Sort = sort;
            }
            //product.Hot = hot;
            product.Active = active;
            product.Home = home;
            _unitOfWork.Save();
            return true;
        }
        #endregion
        public JsonResult GetProductCategory(int? parentId)
        {
            var categories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(categories.Select(a => new { a.Id, Name = a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChildCategory(int parentId = 0)
        {
            var childCategories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(childCategories.Select(a => new { a.Id, a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}