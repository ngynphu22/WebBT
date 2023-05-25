using BinhTung.DAL;
using BinhTung.Models;
using BinhTung.ViewModel;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BinhTung.Controllers
{
    [Authorize]
    public class VcmsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        #region Admin
        public ActionResult Index()
        {
            var model = new InfoAdminViewModel
            {
                Articles = _unitOfWork.ArticleRepository.GetQuery(),
                Banners = _unitOfWork.BannerRepository.GetQuery(),
                Contacts = _unitOfWork.ContactRepository.GetQuery(),
                Feedbacks = _unitOfWork.FeedbackRepository.GetQuery(),
                Products = _unitOfWork.ProductRepository.GetQuery(),
                Admins = _unitOfWork.AdminRepository.Get()
            };
            return View(model);
        }
        [ChildActionOnly]
        public PartialViewResult ListAdmin()
        {
            var admins = _unitOfWork.AdminRepository.Get();
            return PartialView("ListAdmin", admins);
        }
        public ActionResult CreateAdmin(string result = "")
        {
            ViewBag.Result = result;
            //if (!User.Identity.Name.Equals("admin"))
            //{
            //    return RedirectToAction("Index");
            //}
            return View();
        }
        [HttpPost]
        public ActionResult CreateAdmin(Admin model)
        {
            //if (!User.Identity.Name.Equals("admin"))
            //{
            //    return RedirectToAction("Index");
            //}
            if (ModelState.IsValid)
            {
                var admin =
                    _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                if (admin != null)
                {
                    ModelState.AddModelError("", @"Tên đăng nhập này có rồi");
                }
                else
                {
                    var hashPass = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    _unitOfWork.AdminRepository.Insert(new Admin { Username = model.Username, Password = hashPass, Active = model.Active });
                    _unitOfWork.Save();
                    return RedirectToAction("CreateAdmin", new { result = "success" });
                }
            }
            return View(model);
        }
        public ActionResult EditAdmin(int adminId = 0)
        {
            //if (!User.Identity.Name.Equals("admin"))
            //{
            //    return RedirectToAction("Index");
            //}
            var admin = _unitOfWork.AdminRepository.GetById(adminId);
            if (admin == null)
            {
                return RedirectToAction("CreateAdmin");
            }
            return View(admin);
        }
        [HttpPost]
        public ActionResult EditAdmin(Admin model)
        {
            //if (!User.Identity.Name.Equals("admin"))
            //{
            //    return RedirectToAction("Index");
            //}
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                if (admin == null)
                {
                    return RedirectToAction("CreateAdmin");
                }
                admin.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                _unitOfWork.Save();
                return RedirectToAction("CreateAdmin", new { result = "update" });
            }
            return View(model);
        }
        public bool DeleteAdmin(string username)
        {
            if (!User.Identity.Name.Equals("admin"))
            {
                return false;
            }
            var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(username)).SingleOrDefault();
            if (admin == null)
            {
                return false;
            }
            if (username == "admin")
            {
                return false;
            }
            _unitOfWork.AdminRepository.Delete(admin);
            _unitOfWork.Save();
            return true;
        }
        public ActionResult ChangePassword(int result = 0)
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                if (admin == null)
                {
                    return HttpNotFound();
                }
                if (HtmlHelpers.VerifyHash(model.OldPassword, "SHA256", admin.Password))
                {
                    admin.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    _unitOfWork.Save();
                    return RedirectToAction("ChangePassword", new { result = 1 });
                }
            }
            return View(model);
        }
        #endregion

        #region Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(AdminLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.Get(a => a.Username == model.Username).SingleOrDefault();

                if (admin != null && HtmlHelpers.VerifyHash(model.Password, "SHA256", admin.Password))
                {
                    //Session["Role"] = admin.Role;
                    FormsAuthentication.SetAuthCookie(model.Username, true);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Vcms");
                }
                ModelState.AddModelError("", @"Tên đăng nhập  hoặc mật khẩu không chính xác.");
            }
            return View(model);
        }
        public RedirectToRouteResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Vcms");
        }
        #endregion


        #region ConfigSite
        public ActionResult ConfigSite(string result = "")
        {
            ViewBag.Result = result;
            var config = _unitOfWork.ConfigSiteRepository.Get().FirstOrDefault();
            return View(config);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ConfigSite(ConfigSite model)
        {
            var config = _unitOfWork.ConfigSiteRepository.Get().FirstOrDefault();
            if (config == null)
            {
                _unitOfWork.ConfigSiteRepository.Insert(model);
            }
            else
            {

                config.Facebook = model.Facebook;
                config.Linkedin = model.Linkedin;
                config.Instagram = model.Instagram;
                config.GoogleMap = model.GoogleMap;
                config.Youtube = model.Youtube;
                config.Title = model.Title;
                config.Description = model.Description;
                config.GoogleAnalytics = model.GoogleAnalytics;
                config.Hotline = model.Hotline;
                config.Email = model.Email;
                config.LiveChat = model.LiveChat;
                config.Twitter = model.Twitter;
                config.Place = model.Place;
                config.AboutText = model.AboutText;
                config.Instagram = model.Instagram;
                config.FileUrl = model.FileUrl;
                config.HistoryUrl = model.HistoryUrl;
                config.VideoUrl = model.VideoUrl;
                config.InfoFooter = model.InfoFooter;

                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        return View(config);
                    }

                    if (file.ContentLength > 4000 * 1024)
                    {
                        ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        return View(config);
                    }

                    var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);
                    var imgPath = "/images/configs/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    config.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                    file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                }

                var file1 = Request.Files["AboutImage"];
                if (file1 != null && file1.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file1.FileName, "jpg|jpeg|png|gif"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        return View(config);
                    }

                    if (file1.ContentLength > 4000 * 1024)
                    {
                        ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        return View(config);
                    }

                    var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file1.FileName);
                    var imgPath = "/images/configs/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    config.AboutImage = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                    file1.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                }

                var file2 = Request.Files["AboutImage"];
                if (file2 != null && file2.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file2.FileName, "jpg|jpeg|png|gif"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        return View(config);
                    }

                    if (file2.ContentLength > 4000 * 1024)
                    {
                        ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        return View(config);
                    }

                    var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file2.FileName);
                    var imgPath = "/images/configs/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    config.AboutImage = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                    file2.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                }

                _unitOfWork.Save();
                HttpContext.Application["ConfigSite"] = config;
                return RedirectToAction("ConfigSite", "Vcms", new { result = "success" });
            }
            return View("ConfigSite", model);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}