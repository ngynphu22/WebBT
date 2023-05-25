using BinhTung.DAL;
using BinhTung.Models;
using BinhTung.ViewModel;
using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace BinhTung.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];
        private IEnumerable<Banner> Banners => _unitOfWork.BannerRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
        private IEnumerable<ArticleCategory> ArticleCategories => _unitOfWork.ArticleCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private IEnumerable<ProductCategory> ProductCategories => _unitOfWork.ProCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        public ActionResult Index()
        {
            var homeCategories =
            _unitOfWork.ProCategoryRepository.Get(a => a.CategoryActive && a.ShowHome, q => q.OrderBy(c => c.CategorySort));
            var itemsHome = homeCategories.Select(a => new HomeViewModel.ItemHomeProduct
            {
                ProductCategory = a,
                Products = _unitOfWork.ProductRepository.Get(l => l.Active && l.Home && (l.ProductCategoryId == a.Id || l.ProductCategory.ParentId == a.Id), q => q.OrderBy(c => c.Sort), 8),
            });

            var specialCategories =
                _unitOfWork.ProCategoryRepository.Get(x => x.CategoryActive && x.Special, q => q.OrderBy(c => c.CategorySort));
            var itemsSpecial = specialCategories.Select(x => new HomeViewModel.ItemSpecialProduct
            {
                ProductCategory = x,
                Products = _unitOfWork.ProductRepository.Get(l => l.Active && l.Home && (l.ProductCategoryId == x.Id || l.ProductCategory.ParentId == x.Id), q => q.OrderBy(c => c.Sort), 8),
            });
            var model = new HomeViewModel
            {
                ItemHomeProducts = itemsHome,
                ItemSpecialProducts = itemsSpecial,
                Banners = Banners,
                Feedbacks = _unitOfWork.FeedbackRepository.Get(l => l.Active, a => a.OrderBy(c => c.Sort), 24)
            };
            return View(model);
        }
        [ChildActionOnly]
        public PartialViewResult Header()
        {
            var model = new HeaderViewModel
            {
                ArticleCategories = ArticleCategories.Where(l => l.ShowMenu),
                ProductCategories = ProductCategories
            };
            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            var model = new FooterViewModel
            {
                ArticleCategories = ArticleCategories.Where(l => l.ParentId == null),
                ProductCategories = ProductCategories,
                Articles = _unitOfWork.ArticleRepository.GetQuery(l => l.Active && l.ArticleCategory.TypePost == TypePost.Support, a => a.OrderByDescending(c => c.CreateDate), 6)
            };
            return PartialView(model);
        }
        [Route("lien-he")]
        public ActionResult Contact()
        {
            return View();
        }
        public PartialViewResult ContactForm()
        {
            return PartialView();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ContactForm(Contact model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            _unitOfWork.ContactRepository.Insert(model);
            _unitOfWork.Save();
            var subject = "binhtungstone.vn - Email liên hệ từ: " + model.Email;
            var body = $"<p>Tên người liên hệ: {model.Fullname},</p>" +
                        $"<p>Địa chỉ email người liên hệ: {model.Email},</p>" +
                        $"<p>Số điện thoại: {model.Mobile},</p>" +
                        $"<p>Địa chỉ: {model.Address},</p>" +
                        $"<p>Nội dung:{model.Body}</p>";

            Task.Run(() => HtmlHelpers.SendEmail("gmail", subject, body, "Binhtung.dathanhhoa@gmail.com", "email-send@vico.vn",
           "email-send@vico.vn", "send@123", "binhtungstone.vn "));

            return Json(new { status = true, msg = "Gửi liên hệ thành công.\nChúng tôi sẽ liên lạc lại với bạn sớm nhất có thể." });
        }
        [Route("danh-muc/{url:regex(^(?!.*(vcms|article|banner|contact|productvcms|uploader)).*$)}", Order = 2)]
        public ActionResult ProductCategory(int? page, string sort, string url)
        {
            var category = ProductCategories.FirstOrDefault(a => a.Url == url);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var products = _unitOfWork.ProductRepository.GetQuery(l => l.Active && l.ProductCategoryId == category.Id || l.ProductCategory.ParentId == category.Id, c => c.OrderByDescending(a => a.Id));
            switch (sort)
            {
                case "name":
                    products = products.OrderBy(a => a.Name);
                    break;
                case "price":
                    products = products.OrderBy(a => a.Price);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(a => a.Price);
                    break;
                case "sort-desc":
                    products = products.OrderByDescending(a => a.Sort);
                    break;
                default:
                    products = products.OrderByDescending(a => a.Id);
                    break;
            }
            var pageNumber = page ?? 1;
            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, 15),
                Category = category,
                Sort = sort,
                Categories = ProductCategories
            };
            return View(model);
        }
        [Route("san-pham/{url}.html", Order = 1)]
        public ActionResult ProductDetail(string url)
        {
            var product = _unitOfWork.ProductRepository.GetQuery(l => l.Active && l.Url == url).FirstOrDefault();
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            var products = _unitOfWork.ProductRepository.GetQuery(l => l.Active && l.ProductCategoryId == product.ProductCategoryId && l.Id != product.Id, a => a.OrderByDescending(c => c.Id), 3);
            var model = new ProductDetailViewModel
            {
                Product = product,
                Products = products
            };
            return View(model);
        }
        [Route("tin-tuc/{url}", Order = 2)]
        public ActionResult ArticleCategory(int? page, string url)
        {
            var category = ArticleCategories.FirstOrDefault(a => a.Url == url);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var article = _unitOfWork.ArticleRepository.GetQuery(l => l.Active && (l.ArticleCategoryId == category.Id || l.ArticleCategory.ParentId == category.Id), c => c.OrderByDescending(a => a.CreateDate));
            var pageNumber = page ?? 1;
            if (article.Count() == 1)
            {
                var fi = article.First();
                return RedirectToAction("ArticleDetail", new { url = fi.Url });
            }
            var model = new CategoryAticleViewModel
            {
                Articles = article.ToPagedList(pageNumber, 8),
                Category = category,
            };
            return View(model);
        }
        [Route("bai-viet/{url}.html", Order = 1)]
        public ActionResult ArticleDetail(string url)
        {
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (article == null)
            {
                return RedirectToAction("Index");
            }
            var articles = _unitOfWork.ArticleRepository.GetQuery(l => l.Active && l.ArticleCategoryId == article.ArticleCategoryId && l.Id != article.Id, c => c.OrderByDescending(a => a.CreateDate), 3);
            var model = new ArticleDetailViewModel
            {
                Article = article,
                Articles = articles,
            };
            return View(model);
        }
        [ChildActionOnly]
        public PartialViewResult MenuProduct()
        {
            var model = new MenuProductViewModel
            {
                ProductCategories = ProductCategories,
                Articles = _unitOfWork.ArticleRepository.GetQuery(l => l.Active, a => a.OrderByDescending(c => c.CreateDate), 3)
            };
            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult MenuArticle()
        {
            var model = new MenuArticleViewModel
            {
                ArticleCategories = ArticleCategories.Where(l => l.ParentId == null),
                Articles = _unitOfWork.ArticleRepository.GetQuery(l => l.Active, a => a.OrderByDescending(c => c.CreateDate), 4)
            };
            return PartialView(model);
        }
        [Route("dat-hang-nhanh")]
        [HttpPost]
        public bool QuickOrder(FormCollection fc)
        {
            try
            {
                var name = fc["FullName"];
                var address = fc["Address"];
                var email = fc["Email"];
                var mobile = fc["Mobile"];
                var content = fc["Content"];
                var proName = fc["ProductName"];
                var proUrl = fc["ProductUrl"];
                var proImg = fc["ProductImg"];
                var body = new StringBuilder();
                body.Append("<p>Xin chào,</p>");
                body.Append("<p>Dưới đây là thông tin liên hệ nhận báo giá website " + Request.Url?.Host + ":</p>");
                body.Append("<p>Sản phẩm: <strong>" + proName + "</strong></p>");
                body.Append("<p>Hình ảnh: <img src='https://" + Request.Url?.Host + "/images/products/" + Dễ sử dụng + "?w=200' /></p>");
                body.Append("<p>Link SP: https://" + Request.Url?.Host + proUrl + "</p>");
                body.Append("<p>Họ và tên: " + name + "</p>");
                body.Append("<p>Di động: " + mobile + "</p>");
                body.Append("<p>Email: " + email + "</p>");
                body.Append("<p>Địa chỉ: " + address + "</p>");
                body.Append("<p>Nội dung: " + content + "</p>");
                body.Append("<p>Ngày liên hệ: " + DateTime.Now.ToString("HH:mm dd/MM/yyyy") + "</p>");
                body.Append("<p>Đây là email tự động vui lòng không phản hồi lại, vì phản hồi lại chúng tôi sẽ không tiếp nhận được thông tin.</p>");
                var subject = "Liên hệ nhận báo giá từ website " + Request.Url?.Host;
                Task.Run(() =>
                {
                    HtmlHelpers.SendEmail("gmail", subject, body.ToString(), "Binhtung.dathanhhoa@gmail.com", Email, Email, Password, "Đặt hàng Online", email, "Binhtung.dathanhhoa@gmail.com");
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        [Route("tim-kiem-san-pham")]
        public ActionResult Search(int? page, int? categoryId, string keywords)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                return RedirectToActionPermanent("Index");
            }

            var products = _unitOfWork.ProductRepository.GetQuery(l => l.Name.Contains(keywords),
                q => q.OrderByDescending(a => a.Sort));

            if (categoryId.HasValue)
            {
                products = products.Where(a => a.ProductCategoryId == categoryId || a.ProductCategory.ParentId == categoryId);
            }
            var pageNumber = page ?? 1;

            var model = new ProductSearchViewModel
            {
                Keywords = keywords,
                Products = products.ToPagedList(pageNumber, 15),
                CategoryId = categoryId
            };
            return View(model);
        }
        [Route("tim-kiem-bai-viet")]
        public ActionResult SearchArticle(int? page, string keywords)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                return RedirectToAction("Index");
            }
            var article = _unitOfWork.ArticleRepository.GetQuery(l => l.Active && l.Subject.Contains(keywords), a => a.OrderByDescending(c => c.CreateDate));
            var pageNumber = page ?? 1;
            var model = new ArticleSearchViewModel
            {
                Keywords = keywords,
                Articles = article.ToPagedList(pageNumber, 10)
            };
            return View(model);
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}