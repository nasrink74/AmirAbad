using SpadStorePanel.Core.Models;
using SpadStorePanel.Infrastructure;
using SpadStorePanel.Infrastructure.Dtos.Product;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Infrastructure.Services;
using SpadStorePanel.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpadStorePanel.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProductService _productService;
        private readonly ProductGroupsRepository _productGroupsRepository;
        private readonly BrandsRepository _brandsRepository;
        private readonly ProductsRepository _productsRepository;
        private readonly ProductGalleriesRepository _productGalleriesRepository;
        private readonly ProductMainFeaturesRepository _productMainFeaturesRepository;
        private readonly ProductFeatureValuesRepository _productFeatureValuesRepository;
        private readonly ProductCommentsRepository _productCommentsRepository;
        private readonly SubFeaturesRepository _subFeaturesRepository;
        private readonly MyDbContext _context;
        private readonly FeaturesRepository _featuresRepo;
        public ShopController(FeaturesRepository featuresRepo,MyDbContext context,SubFeaturesRepository subFeaturesRepository,ProductCommentsRepository productCommentsRepository,ProductFeatureValuesRepository productFeatureValuesRepository,ProductMainFeaturesRepository productMainFeaturesRepository,ProductGalleriesRepository productGalleriesRepository,ProductsRepository productsRepository,BrandsRepository brandsRepository,ProductGroupsRepository productGroupsRepository,ProductService productService)
        {
            _productService = productService;
            _productGroupsRepository = productGroupsRepository;
            _brandsRepository = brandsRepository;
            _productsRepository = productsRepository;
            _productFeatureValuesRepository = productFeatureValuesRepository;
            _productMainFeaturesRepository = productMainFeaturesRepository;
            _productGalleriesRepository = productGalleriesRepository;
            _productCommentsRepository = productCommentsRepository;
            _subFeaturesRepository = subFeaturesRepository;
            _context = context;
            _featuresRepo = featuresRepo;
        }

        //public ActionResult Index(int page = 1)
        //{
        //    int paresh = (page - 1) * 3;
        //    //تعداد کل ردیف ها
        //    int totalCount = _productService.GetProductsWithPrice().Count();
        //    ViewBag.PageID = page;
        //    double remain = totalCount % 3;

        //    if (remain == 0)
        //    {
        //        ViewBag.PageCount = totalCount / 3;
        //    }
        //    else
        //    {
        //        ViewBag.PageCount = (totalCount / 3) + 1;
        //    }
        //    ViewBag.TotalCount = totalCount;
        //    var model = _productService.GetProductsWithPrice().OrderByDescending(x => x.Id).Skip(paresh).Take(3).ToList();
        //    return View(model);
        //}



        // GET: Products
        [Route("Shop/")]
        [Route("Shop/ProductList/{id}/{title}")]
        [Route("Shop/ProductList/{id}")]
        [Route("Shop/ProductList")]
        [Route("Shop/ProductList/Search/{searchString}")]
        public ActionResult Index(int? id, string searchString = null)

        {
            var vm = new ProductListViewModel();
            vm.SelectedGroupId = id ?? 0;
            var productGroups = new List<ProductGroup>();
            if (id == null)
            {
                //vm.Features = _featuresRepo.GetAllFeatures();
                vm.Brands = _brandsRepository.GetAll();

                var childrenGroups =_productGroupsRepository.GetChildrenProductGroups();
                vm.ProductGroups = childrenGroups;
                ViewBag.Title = "محصولات";
            }
            else
            {
                //vm.Features = _featuresRepo.GetAllGroupFeatures(id.Value);
                vm.Brands = _brandsRepository.GetAllGroupBrands(id.Value);
                var selectedProductGroup = _productGroupsRepository.Get(id.Value);
                var childrenGroups = _productGroupsRepository.GetChildrenProductGroups(id.Value);
                vm.ProductGroups = childrenGroups;
                ViewBag.ProductGroupName = selectedProductGroup.Title;
                ViewBag.ProductGroupId = selectedProductGroup.Id;
                ViewBag.Title = $"محصولات {selectedProductGroup.Title}";
            }

            ViewBag.SearchString = searchString;
            return View(vm);
        }

        [HttpPost]
        public string RegisterCommentProduct(ProductComment comment)
        {


            if (ModelState.IsValid)
            {

                comment.AddedDate = DateTime.Now;
                _context.ProductComments.Add(comment);
                _context.SaveChanges();
                //_productCommentsRepository.Add(comment);
                return "success";
            }
            return "fail";
        }

        public ActionResult SendCommentProduct(int ProductId)
        {
            ViewBag.ProductId = ProductId;
            return View();
        }
        public ActionResult ShowCommentProduct(int ProductId)
        {
            var productComments = _productCommentsRepository.GetAll().Where(a => a.ProductId == ProductId).ToList();
            return View(productComments);
        }

        [Route("ProductsGrid")]
       
        public ActionResult ProductsGrid(GridViewModel grid)
        {
            var products = new List<Product>();

            var brandsIntArr = new List<int>();

            if (string.IsNullOrEmpty(grid.brands) == false)
            {
                var brandsArr = grid.brands.Split('-').ToList();
                brandsArr.ForEach(b => brandsIntArr.Add(Convert.ToInt32(b)));
            }

            //var subFeaturesIntArr = new List<int>();
            //if (string.IsNullOrEmpty(grid.subFeatures) == false)
            //{
            //    var subFeaturesArr = grid.subFeatures.Split('-').ToList();
            //    subFeaturesArr.ForEach(b => subFeaturesIntArr.Add(Convert.ToInt32(b)));
            //}

            products = _productService.GetProductsGrid(grid.categoryId, brandsIntArr, grid.priceFrom, grid.priceTo, grid.searchString);

            #region Sorting

            if (grid.sort != "date")
            {
                switch (grid.sort)
                {
                    case "name":
                        products = products.OrderBy(p => p.Title).ToList();
                        break;
                    case "sale":
                        products = products.OrderByDescending(p => _productService.GetProductSoldCount(p)).ToList();
                        break;
                    case "price-high-to-low":
                        products = products.OrderByDescending(p => _productService.GetProductPriceAfterDiscount(p)).ToList();
                        break;
                    case "price-low-to-high":
                        products = products.OrderBy(p => _productService.GetProductPriceAfterDiscount(p)).ToList();
                        break;
                    case "Name":
                        products = products.OrderBy(p => p.Title).ToList();
                        break;
                    case "Price":
                        products = products.OrderBy(p => _productService.GetProductPriceAfterDiscount(p)).ToList();
                        break;


                }
            }
            #endregion

            var count = products.Count;
            var skip = grid.pageNumber * 3 - 3;
            int pageCount = (int)Math.Ceiling((double)count / 3);
            ViewBag.PageCount = pageCount;
            ViewBag.CurrentPage = grid.pageNumber;
            ViewBag.TotalCount = count;

            products = products.Skip(skip).Take(3).ToList();
            ViewBag.Countproduct = products.Count;
            var vm = new List<ProductWithPriceDto>();
            foreach (var product in products)
                vm.Add(_productService.CreateProductWithPriceDto(product));

            return PartialView(vm);

       
        }
        public ActionResult SizeSection(int? ProductGroupId)
        {
            var ProductSize= _subFeaturesRepository.GetSubFeatures(1003);
            ViewBag.ProductGroupId = ProductGroupId;
            return View(ProductSize);
        }
        public ActionResult GetBySize(int SizeId,int ProductGroupId, int page = 1)
        {
           var p= _productMainFeaturesRepository.productMainFeatures(SizeId, ProductGroupId);
            int paresh = (page - 1) * 3;
            //تعداد کل ردیف ها
            int totalCount = _productService.GetProductsWithPriceSize(p).Count();
            ViewBag.PageID = page;
            double remain = totalCount % 3;

            if (remain == 0)
            {
                ViewBag.PageCount = totalCount / 3;
            }
            else
            {
                ViewBag.PageCount = (totalCount / 3) + 1;
            }
            ViewBag.TotalCount = totalCount;
            ViewBag.ProductGroupId = ProductGroupId;
            ViewBag.SizeId = SizeId;
            var model = _productService.GetProductsWithPriceSize(p).OrderByDescending(x => x.Id).Skip(paresh).Take(3).ToList();
            return View("Index", model);

        }
        public ActionResult PriceSection(int? ProductGroupId)
        {
            ViewBag.ProductGroupId = ProductGroupId;
            return View();
        }
        public ActionResult PriceFilter(int?ProductGroupId,int max, int min, int page = 1)
        {

            int paresh = (page - 1) * 3;
            //تعداد کل ردیف ها
            int totalCount = _productService.GetProductsWithPrice().Where(x => x.Price < max && x.Price > min).Count();
            ViewBag.PageID = page;
            double remain = totalCount % 3;

            if (remain == 0)
            {
                ViewBag.PageCount = totalCount / 3;
            }
            else
            {
                ViewBag.PageCount = (totalCount / 3) + 1;
            }
            ViewBag.TotalCount = totalCount;
            ViewBag.ProductGroupId = ProductGroupId;
            ViewBag.min = min;
            ViewBag.max = max;
            var model = _productService.GetProductsWithPrice().Where(x => x.Price < max && x.Price > min).OrderByDescending(x => x.Id).Skip(paresh).Take(3).ToList();
            return View("Index", model);
        }
        public ActionResult BrandSection(int? ProductGroupId)
        {
           var brands= _brandsRepository.brands(ProductGroupId).ToList();
            ViewBag.ProductGroupId = ProductGroupId;
            return View(brands);
        }
        public ActionResult GetByBrand(int BrandId, int? ProductGroupId, int page = 1)
        {

            int paresh = (page - 1) * 3;
            //تعداد کل ردیف ها
            int totalCount = _productService.GetProductsWithPrice().Where(x => x.BrandId == BrandId).Count();
            ViewBag.PageID = page;
            double remain = totalCount % 3;

            if (remain == 0)
            {
                ViewBag.PageCount = totalCount / 3;
            }
            else
            {
                ViewBag.PageCount = (totalCount / 3) + 1;
            }
            ViewBag.TotalCount = totalCount;
            ViewBag.ProductGroupId = ProductGroupId;
            ViewBag.BrandId = BrandId;
            var model = _productService.GetProductsWithPrice().Where(x => x.BrandId == BrandId).OrderByDescending(x => x.Id).Skip(paresh).Take(3).ToList();
            return View("Index", model);
        }
        public ActionResult Name(int page = 1)
        {
            int paresh = (page - 1) * 3;
            //تعداد کل ردیف ها
            int totalCount = _productService.GetProductsWithPrice().Count();
            ViewBag.PageID = page;
            double remain = totalCount % 3;

            if (remain == 0)
            {
                ViewBag.PageCount = totalCount / 3;
            }
            else
            {
                ViewBag.PageCount = (totalCount / 3) + 1;
            }
            ViewBag.TotalCount = totalCount;
            var model = _productService.GetProductsWithPrice().OrderByDescending(x => x.ShortTitle).Skip(paresh).Take(3).ToList();
            return View("Index", model);
        }
        public ActionResult Price(int page=1)
        
            {
            int paresh = (page - 1) * 3;
            //تعداد کل ردیف ها
            int totalCount = _productService.GetProductsWithPrice().Count();
            ViewBag.PageID = page;
            double remain = totalCount % 3;

            if (remain == 0)
            {
                ViewBag.PageCount = totalCount / 3;
            }
            else
            {
                ViewBag.PageCount = (totalCount / 3) + 1;
            }
            ViewBag.TotalCount = totalCount;
            var model = _productService.GetProductsWithPrice().OrderByDescending(x => x.Price).Skip(paresh).Take(3).ToList();
            return View("Index", model);
        }


        public ActionResult GetTags()
        {
            return View(_context.ArticleTags.Where(a => a.IsDeleted == false).ToList());
        }
        public ActionResult ProductGroupSection()
        {
            var ProductGroup = _productGroupsRepository.GetAll().Where(a => a.IsDeleted == false).OrderByDescending(a => a.InsertDate).ToList();
            return View(ProductGroup);
        }
        public ActionResult Detail(int id)
        {
            var product = _productsRepository.GetProduct(id);
            var productGallery = _productGalleriesRepository.GetProductGalleries(id);
            var productMainFeatures = _productMainFeaturesRepository.GetProductMainFeature(id);
            var productFeatureValues = _productFeatureValuesRepository.GetProductFeatures(id);
            var price = _productService.GetProductPrice(product);
            var priceAfterDiscount = _productService.GetProductPriceAfterDiscount(product);
            var Productcomments = _productCommentsRepository.GetProductComments(id);


            var vm = new ProductDetailViewModel()
            {
                Product = product,
                ProductGalleries = productGallery,
                ProductMainFeatures = productMainFeatures,
                ProductFeatureValues = productFeatureValues,
                Price = price,
                Comments = Productcomments,
                PriceAfterDiscount = priceAfterDiscount,
                DiscountPercentage = (int)(priceAfterDiscount * 100 / price)
            };
            return View(vm);
        }

        public ActionResult RelatedProduct(int? ProductGroupId,int productid)
        {
            var model = _productService.GetProductsWithPrice().Where(a => a.ProductGroupId == ProductGroupId &a.Id!= productid).OrderByDescending(x => x.DateTime).Skip(0).Take(10).ToList();
            return View(model);
        }

        public ActionResult Favorit(int idGroupId,int idproduct)
        {
          var model=  _productService.GetProductsWithPrice().Where(a => a.ProductGroupId == idGroupId && a.Id!= idproduct).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult NextProduct(int id)
        {

            List<Product> AllProduct = _productsRepository.GetAll().Where(a => a.IsDeleted == false).ToList();

            int found = 0;
            for (int i = 0; i < AllProduct.Count(); i++)
            {
                if (AllProduct[i].Id == id)
                {
                    found = i;
                    break;
                }
            }
            if (found != (AllProduct.Count() - 1))
            {

                return RedirectToAction("Detail", AllProduct[found + 1]);
            }

            return RedirectToAction("Detail","Shop", AllProduct[found]);

        }

        [HttpGet]
        public ActionResult PreviousProduct(int id)
        {
            List<Product> AllProduct = _productsRepository.GetAll().Where(a => a.IsDeleted == false).ToList();

            int found = 0;
            for (int i = 0; i < AllProduct.Count(); i++)
            {
                if (AllProduct[i].Id == id)
                {
                    found = i;
                    break;
                }
            }
            if (found != 0)
            {
              
                return RedirectToAction("Detail", AllProduct[found - 1]);
            }

            return RedirectToAction("Detail", "Shop", AllProduct[found]);

        }

        public ActionResult Search(String txtsearch, int page = 1)
        {
            if (!string.IsNullOrEmpty(txtsearch))
            {
         
                int paresh = (page - 1) * 3;
                //تعداد کل ردیف ها
                int totalCount = _productService.GetSearchProductsWithPrice(txtsearch).Count();
                ViewBag.PageID = page;
                double remain = totalCount % 3;

                if (remain == 0)
                {
                    ViewBag.PageCount = totalCount / 3;
                }
                else
                {
                    ViewBag.PageCount = (totalCount / 3) + 1;
                }
                ViewBag.searchVal = txtsearch;
                ViewBag.TotalCount = totalCount;

                var model = _productService.GetSearchProductsWithPrice(txtsearch).OrderByDescending(x => x.Id).Skip(paresh).Take(3).ToList();
                if (model.Count() == 0)
                {
                    return RedirectToAction("NotFoundSearch");
                }
                return View("Index",model);



            }
            else return RedirectToAction("Index");
        }
        public ActionResult NotFoundSearch()
        {
            return View();
        }
    }
}