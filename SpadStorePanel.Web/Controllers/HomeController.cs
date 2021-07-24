using SpadStorePanel.Core.Models;
using SpadStorePanel.Infrastructure;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Infrastructure.Services;
using SpadStorePanel.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpadStorePanel.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly StaticContentDetailsRepository _staticContentDetailsRepository;
        private readonly FaqRepository _faqRepository;
        private readonly UsersRepository _repo;
        private readonly MyDbContext _context;
        private readonly ProductGroupsRepository _productGroupsRepository;
        private readonly ContactFormsRepository _contactFormsRepository;
        private readonly ArticlesRepository _articlesRepo;
        private readonly ProductService _productService;
        private readonly OurTeamRepository _ourTeamRepository;
        public HomeController(OurTeamRepository ourTeamRepository,ProductService productService,ArticlesRepository articlesRepo,ContactFormsRepository contactFormsRepository, ProductGroupsRepository productGroupsRepository,MyDbContext context,UsersRepository repo,FaqRepository faqRepository,StaticContentDetailsRepository staticContentDetailsRepository)
        {
            _staticContentDetailsRepository = staticContentDetailsRepository;
            _faqRepository = faqRepository;
            _repo = repo;
            _context = context;
            _productGroupsRepository = productGroupsRepository;
            _contactFormsRepository = contactFormsRepository;
            _articlesRepo = articlesRepo;
            _productService = productService;
            _ourTeamRepository = ourTeamRepository;
        }
        public ActionResult Index()
        {
          
            ////return Redirect("/Admin/Dashboard");
            return View();
        }
        public ActionResult HomeHistory()
        {

            var Histoty = _staticContentDetailsRepository.GetAll().Where(a => a.Id == 16 && a.IsDeleted == false).FirstOrDefault();
            return View(Histoty);
        }
        public ActionResult HomeTeam()
        {

            var OrTeam = _ourTeamRepository.GetAll().Where(a => a.IsDeleted == false).ToList();
            return View(OrTeam);
        }
 

        public ActionResult HomeProductGroup()
        {
            var ProductGroup = _productGroupsRepository.GetAll().Where(a => a.IsDeleted == false & a.ParentId == null).OrderByDescending(a => a.InsertDate).ToList();
            return View(ProductGroup);
            ////return Redirect("/Admin/Dashboard");
            //return View();
        }
        public ActionResult testt()
        {
            ////return Redirect("/Admin/Dashboard");
            return View();
        }
        public ActionResult HeaderMobile()
        {
            ////return Redirect("/Admin/Dashboard");
            return View();
        }
        

        public ActionResult ContactUsSummary()
        {
            return View();
        }
        public ActionResult HomeHeaderSection()
        {


            try
            {
                var cartModel = new CartModel();
                cartModel.CartItems = new List<CartItemModel>();

                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                ViewBag.Phone = _staticContentDetailsRepository.Get(9).Description;
                ViewBag.Address = _staticContentDetailsRepository.Get(7).Description;
                ViewBag.WorkTime = _staticContentDetailsRepository.Get(48).Description;
                return View(cartModel);

            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;


                var cartModel = new CartModel();
                cartModel.CartItems = new List<CartItemModel>();

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                ViewBag.Phone = _staticContentDetailsRepository.Get(9).Description;
                ViewBag.Address = _staticContentDetailsRepository.Get(7).Description;
                ViewBag.WorkTime = _staticContentDetailsRepository.Get(48).Description;
                return View(cartModel);

            }

        }
        public ActionResult HeaderSection()
        {
           
         
            try
            {
                var cartModel = new CartModel();
                cartModel.CartItems = new List<CartItemModel>();

                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                ViewBag.Phone = _staticContentDetailsRepository.Get(9).Description;
                ViewBag.Address = _staticContentDetailsRepository.Get(7).Description;
                ViewBag.WorkTime = _staticContentDetailsRepository.Get(48).Description;
                return View(cartModel);

            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;


                var cartModel = new CartModel();
                cartModel.CartItems = new List<CartItemModel>();

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                ViewBag.Phone = _staticContentDetailsRepository.Get(9).Description;
                ViewBag.Address = _staticContentDetailsRepository.Get(7).Description;
                ViewBag.WorkTime = _staticContentDetailsRepository.Get(48).Description;
                return View(cartModel);

            }
  
        }

        public ActionResult FooterSection()
        {
            FooterViewModel model = new FooterViewModel();
            model.Phone = _staticContentDetailsRepository.Get(9).Description;

            model.Email = _staticContentDetailsRepository.Get(8).Description;

            model.Address = _staticContentDetailsRepository.Get(7).Description;
            model.WorkTime = _staticContentDetailsRepository.Get(48).Description;
            model.ConstText= _staticContentDetailsRepository.Get(47).Description;

            return View(model);
        }

        public ActionResult BlogFooter()
        {
            var articles = new List<Article>();

            articles = _articlesRepo.GetArticles();
            var articlelistVm = new List<ArticleListViewModel>();
            foreach (var item in articles)
            {
                var vm = new ArticleListViewModel(item);
                vm.Role = _articlesRepo.GetAuthorRole(item.UserId);
                vm.GroupArticleName = _articlesRepo.GetCategory(item.ArticleCategoryId.Value).Title;
                vm.CountVoute = item.ArticleComments.Count();
                articlelistVm.Add(vm);
            }

            //ViewBag.ArticleTags = _articlesRepo.GetArticleTags(articles.Id);
            return View(articlelistVm.Skip(0).Take(2).ToList());
        }

        public ActionResult SocialNetwork()
        {
            SocialViewModel socialViewModel = new SocialViewModel();

            FooterViewModel model = new FooterViewModel(socialViewModel);

            model._SocialLinks.Facebook = _staticContentDetailsRepository.Get(5).Link;

            model._SocialLinks.Linkdin = _staticContentDetailsRepository.Get(6).Link;

            model._SocialLinks.GooglePlus = _staticContentDetailsRepository.Get(41).Link;

            model._SocialLinks.Pintrest = _staticContentDetailsRepository.Get(42).Link;

            model._SocialLinks.twitter = _staticContentDetailsRepository.Get(12).Link;

            model.Phone = _staticContentDetailsRepository.Get(9).Description;

            model.Email = _staticContentDetailsRepository.Get(8).Description;

            model.Address = _staticContentDetailsRepository.Get(7).Description;
            return View(model);
        }

        public ActionResult Faq()
        {
           var FaqList= _faqRepository.GetAll().Where(a => a.IsDeleted == false).ToList();
            return View(FaqList);

        }
     
     

        public ActionResult Gallery()
        {
           var Gallry= _productService.GetProductsWithPrice().OrderByDescending(x => x.Price).ToList();
            return View(Gallry);
        }

        public ActionResult SliderShowSection()
        {
            var SliderShow=_staticContentDetailsRepository.GetAll().Where(a => a.StaticContentTypeId == 15 && a.IsDeleted==false).ToList();
            return View(SliderShow);
        }
        public ActionResult SliderConstSection()
        {
            var SliderConst = _staticContentDetailsRepository.GetAll().Where(a => a.StaticContentTypeId == 16 && a.IsDeleted == false).ToList();
            var sliders = new List<StaticContentDetail>();
            foreach(var item in SliderConst)
            {
                sliders.Add(item);
            }
           

            return View(sliders);
        }
        
        public ActionResult ProductGroup()
        {
           var ProductGroup= _productGroupsRepository.GetAll().Where(a => a.IsDeleted == false & a.ParentId==null).OrderByDescending(a=>a.InsertDate).ToList();
            return View(ProductGroup);
        }
        public ActionResult SubscribeInNews()
        {
            return View();
        }
        //[HttpPost]
        //public string RegisterEmail(ContactForm contactForm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        #region Check for duplicate username or email
        //        if (_contactFormsRepository.GetAll().Where(a=>a.Email== contactForm.Email).FirstOrDefault()!=null)
        //        {
        //            return "faill";
        //            //ViewBag.Message = "کاربر دیگری با همین ایمیل در سیستم ثبت شده";
        //            //return View(form);
        //        }
        //        #endregion
        //        _contactFormsRepository.Add(contactForm);
        //        //return Json(success, JsonRequestBehavior.AllowGet);
        //        return "success";
        //    }
        //    else
        //    {
        //        return "fail";
        //    }


        //}

        [HttpPost]
        public string RegisterEmail(ContactForm contactForm)
        {
            if (ModelState.IsValid)
            {
                #region Check for duplicate username or email
                if (_contactFormsRepository.GetAll().Where(a => a.Email == contactForm.Email).FirstOrDefault() != null)
                {
                    return "faill";
                    //ViewBag.Message = "کاربر دیگری با همین ایمیل در سیستم ثبت شده";
                    //return View(form);
                }
                #endregion
                _context.ContactForms.Add(contactForm);
                _context.SaveChanges();
               // _contactFormsRepository.Add(contactForm);
                //return Json(success, JsonRequestBehavior.AllowGet);
                return "success";
            }
            else
            {
                return "fail";
            }


        }
        public ActionResult Service()
        {
            return View();
        }

        public ActionResult TwoImage()
        {
            if (_staticContentDetailsRepository.GetAll().Where(a => a.Id == 32 && a.IsDeleted == false).FirstOrDefault() != null)
            ViewBag.Image1= _staticContentDetailsRepository.GetAll().Where(a => a.Id == 32 && a.IsDeleted==false).FirstOrDefault();
        if(_staticContentDetailsRepository.GetAll().Where(a => a.Id == 33 && a.IsDeleted == false).FirstOrDefault()!=null)
            ViewBag.Image2= _staticContentDetailsRepository.GetAll().Where(a => a.Id == 33 && a.IsDeleted == false).FirstOrDefault();
            return View();
        }

        public ActionResult PopularProducts()
        {
            var model = _productService.GetProductsWithPrice().OrderByDescending(x => x.Rate).Skip(0).Take(10).ToList();
            return View(model);
        }
        public ActionResult NewProducts(int? ProductGroupId)
        {
            var model = _productService.GetProductsWithPrice().Where(a=>a.ProductGroupId== ProductGroupId).OrderByDescending(x => x.DateTime).Skip(0).Take(10).ToList();
            return View(model);
        }
        public ActionResult HomeNewProducts()
        {
            var model = _productService.GetProductsWithPrice().OrderByDescending(x => x.DateTime).Skip(0).Take(6).ToList();
            return View(model);
        }
        public ActionResult SpecialProducts()
        {
            var model = _productService.GetProductsWithPrice().OrderByDescending(x => x.DateTime).Skip(0).Take(6).ToList();
            return View(model);
        }
        public ActionResult BestsellersProducts()
        {
            var model = _productService.GetProductsWithPrice().OrderByDescending(x => x.DateTime).Skip(0).Take(10).ToList();
            return View(model);
        }
       


        public ActionResult bannerad()
        {
           var Banner= _staticContentDetailsRepository.GetAll().Where(a => a.Id == 27 && a.IsDeleted == false).FirstOrDefault();
            return View(Banner);
        }

        public ActionResult NewArticle()
        {
            var articles = new List<Article>();
           
                articles = _articlesRepo.GetArticles();
            var articlelistVm = new List<ArticleListViewModel>();
            foreach (var item in articles)
            {
                var vm = new ArticleListViewModel(item);
                vm.Role = _articlesRepo.GetAuthorRole(item.UserId);
                vm.GroupArticleName = _articlesRepo.GetCategory(item.ArticleCategoryId.Value).Title;
                vm.CountVoute = item.ArticleComments.Count();
                articlelistVm.Add(vm);
            }

            //ViewBag.ArticleTags = _articlesRepo.GetArticleTags(articles.Id);
            return View(articlelistVm.Skip(0).Take(3));
        }
        public ActionResult Appreciation()
        {
            if (_staticContentDetailsRepository.GetAll().Where(a => a.StaticContentTypeId == 6 && a.IsDeleted == false).ToList().Count() > 0)
            {
                var appreciation = _staticContentDetailsRepository.GetAll().Where(a => a.StaticContentTypeId == 6 && a.IsDeleted == false).ToList();
                return View(appreciation);
            }
            return View();

        }

       

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string vImagePath = String.Empty;
            string vMessage = String.Empty;
            string vFilePath = String.Empty;
            string vOutput = String.Empty;
            try
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var vFileName = DateTime.Now.ToString("yyyyMMdd-HHMMssff") +
                                    Path.GetExtension(upload.FileName).ToLower();
                    var vFolderPath = Server.MapPath("/Upload/");
                    if (!Directory.Exists(vFolderPath))
                    {
                        Directory.CreateDirectory(vFolderPath);
                    }
                    vFilePath = Path.Combine(vFolderPath, vFileName);
                    upload.SaveAs(vFilePath);
                    vImagePath = Url.Content("/Upload/" + vFileName);
                    vMessage = "Image was saved correctly";
                }
            }
            catch
            {
                vMessage = "There was an issue uploading";
            }
            vOutput = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + vImagePath + "\", \"" + vMessage + "\");</script></body></html>";
            return Content(vOutput);
        }

  
    }
}