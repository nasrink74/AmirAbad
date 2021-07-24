using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SpadStorePanel.Core.Models;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class InvoicesRepository : BaseRepository<Invoice, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        
        public InvoicesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
           
        }
        public List<InvoiceItem> GetInvoiceItems(int InvoicId)
        {
           var InvoiceItems=  _context.InvoiceItems.Include(a=>a.Product).Include(a=>a.Product.ProductMainFeatures).Where(a => a.InvoiceId == InvoicId).ToList();
            return InvoiceItems;
        }

        public List<Invoice> GetInvoices()
        {
            return _context.Invoices.Include(i => i.Customer.User).ToList();
        }
        public List<Invoice> GetInvoicesCustomer(int customerid)
        {
            return _context.Invoices.Include(i => i.Customer.User).Where(a => a.CustomerId == customerid).ToList();
        }

        public Invoice GetInvoice(int invoiceId)
        {
            return _context.Invoices.Include(i => i.Customer.User).Include(i=>i.InvoiceItems).FirstOrDefault(i=>i.Id == invoiceId);
        }

        public string GetInvoiceItemsMainFeature(int invoiceItemId)
        {
            var invoiceItem = _context.InvoiceItems.Find(invoiceItemId);
            var mainFeature = _context.ProductMainFeatures.Include(m=>m.SubFeature).FirstOrDefault(m=>m.Id == invoiceItem.MainFeatureId);
            return mainFeature.SubFeature.Value;
        }
        public List<Product> GertTopSoldProducts(int take)
        {
            List<Product> products = new List<Product>();
            var productIds = _context.InvoiceItems.GroupBy(i => i.ProductId)
                .OrderByDescending(pi => pi.Count())
                .Select(g => g.Key).ToList();
            foreach (var id in productIds)
            {
                if (products.Count < take)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == id);
                    if (product != null && product.IsDeleted == false)
                    {
                        products.Add(product);
                    }
                }
            }

            return products;
        }

        //public Invoice AddInvoice()
        //{
        //    Invoice invoice = new Invoice();
        //    foreach (var item in cartModel)
        //    {
        //        item.
        //    }
        //    //invoice.TotalPrice=cartModel.
        //}
    }

  
}
