using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Core.Utility;
using SpadStorePanel.Infrastructure;
using SpadStorePanel.Infrastructure.Dtos.Product;
using SpadStorePanel.Infrastructure.Helpers;
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
    public class CookieController : Controller
    {
        private readonly ProductService _productService;

        private readonly ProductMainFeaturesRepository _productMainFeaturesRepo;

        private readonly ProductsRepository _productsRepository;
        private readonly UsersRepository _repo;
        private readonly GeoDivisionsRepository _geoDivisonsRepo;
        private readonly MyDbContext _context;
        private readonly CustomersRepository _customersRepository;
        private readonly InvoicesRepository _invoicesRepository;




        public CookieController(InvoicesRepository invoicesRepository, CustomersRepository customersRepository, GeoDivisionsRepository geoDivisonsRepo, MyDbContext context, UsersRepository repo, ProductService productService, ProductMainFeaturesRepository productMainFeaturesRepository, ProductsRepository productsRepository)
        {
            _productService = productService;

            _productMainFeaturesRepo = productMainFeaturesRepository;

            _productsRepository = productsRepository;
            _repo = repo;
            _context = context;
            _geoDivisonsRepo = geoDivisonsRepo;
            _customersRepository = customersRepository;
            _invoicesRepository = invoicesRepository;


        }

        [HttpPost]
        public void AddToCart(int productId, int? mainFeatureId)
        {
            try
            {
                var cartModel = new CartModel();
                var cartItemsModel = new List<CartItemModel>();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                    cartItemsModel = cartModel.CartItems;
                }
                #endregion

                ProductWithPriceDto product;
                int productStockCount;
                if (mainFeatureId == null)
                {
                    mainFeatureId = _productMainFeaturesRepo.GetByProductId(productId).Id;
                }
                product = _productService.CreateProductWithPriceDto(productId, mainFeatureId.Value);
                productStockCount = _productService.GetProductStockCount(productId, mainFeatureId.Value);

                if (productStockCount > 0)
                {
                    if (cartItemsModel.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value))
                    {
                        if (cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity < productStockCount)
                        {
                            cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity += 1;
                            cartModel.TotalPrice += product.PriceAfterDiscount;
                        }
                    }
                    else
                    {
                        cartItemsModel.Add(new CartItemModel()
                        {
                            Id = product.Id,
                            ProductName = product.ShortTitle,
                            Price = product.PriceAfterDiscount,
                            Quantity = 1,
                            MainFeatureId = mainFeatureId.Value,
                            Image = product.Image
                        });
                        cartModel.TotalPrice += product.PriceAfterDiscount;
                    }
                    cartModel.CartItems = cartItemsModel;
                    var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                    cartCookie.Values.Set("cart", jsonStr);

                    cartCookie.Expires = DateTime.Now.AddHours(12);
                    cartCookie.SameSite = SameSiteMode.Lax;
                    Response.Cookies.Add(cartCookie);
                }
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }

        public ActionResult CartSection()
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
                return PartialView(cartModel);

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
                return PartialView(cartModel);

            }
        }

        [HttpPost]
        public void RemoveFromCart(int productId, int? mainFeatureId, string complete = null)
        {
            try
            {
                var cartModel = new CartModel();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                #endregion

                if (cartModel.CartItems.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId))
                {
                    var itemToRemove = cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId);
                    if (complete == "true" || itemToRemove.Quantity < 2)
                    {
                        cartModel.TotalPrice -= itemToRemove.Price * itemToRemove.Quantity;
                        cartModel.CartItems.Remove(itemToRemove);
                    }
                    else if (complete == "false")
                    {
                        cartModel.TotalPrice -= itemToRemove.Price;
                        cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId).Quantity -= 1;
                    }
                }
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                cartCookie.Values.Set("cart", jsonStr);
                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }


        public void AddToWishList(int productId)
        {
            try
            {
                var withListModel = new WishListModel();
                var withListItemsModel = new List<WishListItemModel>();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

                if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
                {
                    string cartJsonStr = cartCookie.Values["wishList"];
                    withListModel = new WishListModel(cartJsonStr);
                    withListItemsModel = withListModel.WishListItems;
                }
                #endregion

                var product = _productsRepository.Get(productId);
                if (withListItemsModel.Any(i => i.Id == productId) == false)
                {
                    withListItemsModel.Add(new WishListItemModel()
                    {
                        Id = product.Id,
                        ProductName = product.Title,
                        Image = product.Image
                    });
                }
                withListModel.WishListItems = withListItemsModel;
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(withListModel);
                cartCookie.Values.Set("wishList", jsonStr);

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("wishList", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }



        [HttpPost]
        public void RemoveFromWishList(int productId)
        {
            try
            {
                var withListModel = new WishListModel();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

                if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
                {
                    string cartJsonStr = cartCookie.Values["wishList"];
                    withListModel = new WishListModel(cartJsonStr);
                }
                #endregion

                if (withListModel.WishListItems.Any(i => i.Id == productId))
                {
                    var itemToRemove = withListModel.WishListItems.FirstOrDefault(i => i.Id == productId);
                    withListModel.WishListItems.Remove(itemToRemove);
                }
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(withListModel);
                cartCookie.Values.Set("wishList", jsonStr);
                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("wishList", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }

        public ActionResult CardShow()
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
                return PartialView(cartModel);

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
                return PartialView(cartModel);

            }
        }

        public ActionResult WishListTable()
        {
            var wishListModel = new WishListModel();

            try
            {
                HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

                if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
                {
                    string cartJsonStr = cartCookie.Values["wishList"];
                    wishListModel = new WishListModel(cartJsonStr);

                }

            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("wishList", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
            return View(wishListModel);


        }

        public ActionResult UserRoles(String userId)
        {
            var Roles = "d5e29fab-8332-4f55-b156-60439cc60dea";
            if (!_repo.UserHasRole(userId, Roles))
            {
                _repo.AddUserRole(userId, Roles);
            }
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string v)
        {
            throw new NotImplementedException();
        }

        public ActionResult CheckOut()
        {
            var UserID = CheckPermission.GetCurrentUserId();
            var user = _context.Users.Where(a => a.Id == UserID).FirstOrDefault();
            var customer = _context.Customers.Where(a => a.UserId == UserID).FirstOrDefault();
            var q = new AddCustomerInvoceViewModel();
            if(user!=null&& customer != null) { 
            q.FirstName = user.FirstName;
            q.LastName = user.LastName;
            q.NationalCode = customer.NationalCode;
            q.GeoDivisionId = customer.GeoDivisionId;
            q.Address = customer.Address;
            q.PostalCode = customer.PostalCode;
            q.PhoneNumber = user.PhoneNumber;
            }
            ViewBag.Message = null;
            ViewBag.GeoDivisionId = new SelectList(_geoDivisonsRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title");
            return View(q);
        }

        public ActionResult ShowInvoce()
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
                return View(cartModel);

            }


        }


        public ActionResult Payment(AddCustomerInvoceViewModel form)
        {
            if (ModelState.IsValid)
            {
                //ثبت اطلاعات مشتری
                var UserID = CheckPermission.GetCurrentUserId();
                var user = _context.Users.Where(a => a.Id == UserID).FirstOrDefault();
                var userModel = new User()
                {
                    Id = UserID,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = form.FirstName,
                    LastName = form.LastName,
                    PhoneNumber = form.PhoneNumber,
                    PasswordHash = user.PasswordHash,
                    SecurityStamp = user.SecurityStamp
                };

                _repo.UpdateUser(userModel);


                var findcustomer = _context.Customers.Where(a => a.UserId == UserID).FirstOrDefault();


                var customer = new Customer()
                {
                    UserId = UserID,
                    IsDeleted = false,
                    NationalCode = form.NationalCode,
                    Address = form.Address,
                    PostalCode = form.PostalCode,
                    GeoDivisionId = form.GeoDivisionId
                };
                if (findcustomer != null)
                {
                    customer.Id = findcustomer.Id;
                    _customersRepository.Update(customer);
                }
                else
                {
                    _customersRepository.Add(customer);
                }



                //عملیات روی سبد خرید

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
                    //ثبت فاکتور
                    Random rand = new Random();
                    var invoice = new Invoice()
                    {
                        TotalPrice = cartModel.TotalPrice,
                        CustomerId = customer.Id,
                        CustomerName = userModel.FirstName,
                        AddedDate = DateTime.Now,
                        InvoiceNumber = rand.Next(0, 10000).ToString(),
                        GeoDivisionId = customer.GeoDivisionId,
                        Address = customer.Address,
                        Phone = userModel.PhoneNumber,
                        IsPayed = true

                    };
                    _invoicesRepository.Add(invoice);

                    //ثبت جزییات فاکتور
                    foreach (var item in cartModel.CartItems)
                    {
                        var invoicitem = new InvoiceItem()
                        {
                            ProductId = item.Id,
                            InvoiceId = invoice.Id,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            TotalPrice = item.Quantity * item.Price,
                            MainFeatureId = item.MainFeatureId
                        };
                        _context.InvoiceItems.Add(invoicitem);
                        _context.SaveChanges();
                        //کم کردن موجودی
                        _productMainFeaturesRepo.UpdateQuantity(item.MainFeatureId, item.Quantity);
                        //حذف سبد خرید
                        string complete = "true";
                        RemoveFromCart(@item.Id, @item.MainFeatureId, complete);
                    }
                    ViewBag.Message = "sucsess";
                    return View();

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
                    ViewBag.Message = "fail";
                    return View();

                }

            }
            ViewBag.Message = null;
            ViewBag.GeoDivisionId = new SelectList(_geoDivisonsRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", form.GeoDivisionId);
            return View("CheckOut", form);

        }
    }
}
