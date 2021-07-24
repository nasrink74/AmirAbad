using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Core.Utility;
using SpadStorePanel.Infrastructure;
using SpadStorePanel.Infrastructure.Helpers;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Web.ViewModels;

namespace SpadStorePanel.Web.Areas.Admin.Controllers
{
    public class InvoicesCustomerController : Controller
    {
        private readonly InvoicesRepository _repo;
        private readonly GeoDivisionsRepository _GeoRepo;
        private readonly CustomersRepository _customersRepository;
        private readonly MyDbContext _context;
        public InvoicesCustomerController(MyDbContext context,CustomersRepository customersRepository,InvoicesRepository repo, GeoDivisionsRepository geoRepo)
        {
            _repo = repo;
            _GeoRepo = geoRepo;
            _customersRepository = customersRepository;
            _context = context;
        }

        // GET: Admin/InvoicesCustomer
        public ActionResult Index()
        {
           var userid= CheckPermission.GetCurrentUserId();
            var Customer = _customersRepository.GetInvoicesCustomer(userid);
            if (Customer != null)
            {
                var InvoicesCustomer = _repo.GetInvoicesCustomer(Customer.Id);
                var vm = new List<InvoiceTableViewModel>();
                foreach (var invoice in InvoicesCustomer)
                {
                    vm.Add(new InvoiceTableViewModel(invoice));
                }
                if (userid != "")
                {
                    var userrole = _context.UserRoles.Where(a => a.UserId == userid).FirstOrDefault();
                    ViewBag.role = userrole.RoleId;
                }
                return View(vm);
            }
            if (userid != "")
            {
                var userrole = _context.UserRoles.Where(a => a.UserId == userid).FirstOrDefault();
                ViewBag.role = userrole.RoleId;
            }
            return View();
            //var vm = new List<InvoiceTableViewModel>();
            //foreach (var invoice in InvoicesCustomer2)
            //{
            //    vm.Add(new InvoiceTableViewModel(invoice));
            //}
            //return View(vm);
        }
    

        public ActionResult ViewInvoice(int invoiceId)
        {
            var userid = CheckPermission.GetCurrentUserId();
            var vm = new ViewInvoiceViewModel();
            var invoice = _repo.GetInvoice(invoiceId);
            vm.Invoice = invoice;
            vm.PersianDate = new PersianDateTime(invoice.AddedDate).ToString();
            vm.InvoiceItems = new List<InvoiceItemWithMainFeatureViewModel>();
            // Getting Invoice Item SubFeatures
            var InvoiceItems =_repo.GetInvoiceItems(invoiceId);
            foreach (var invoiceItem in InvoiceItems)
            {
                var invoiceItemWithMainFeature = new InvoiceItemWithMainFeatureViewModel
                {
                    InvoiceItem = invoiceItem,
                    MainFeature = _repo.GetInvoiceItemsMainFeature(invoiceItem.Id)
                };
                vm.InvoiceItems.Add(invoiceItemWithMainFeature);

            }
            if (userid != "")
            {
                var userrole = _context.UserRoles.Where(a => a.UserId == userid).FirstOrDefault();
                ViewBag.role = userrole.RoleId;
            }
            return View(vm);
        }
  

 

    }
}