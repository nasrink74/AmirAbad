using SpadStorePanel.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class ProductMainFeaturesRepository : BaseRepository<ProductMainFeature, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public ProductMainFeaturesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public ProductMainFeature GetByProductId(int productId)
        {
            return _context.ProductMainFeatures.FirstOrDefault(f => f.IsDeleted == false && f.ProductId == productId);
        }
        public ProductMainFeature GetByProductId(int productId, int mainFeatureId)
        {
            return _context.ProductMainFeatures.FirstOrDefault(f => f.IsDeleted == false && f.ProductId == productId && f.Id == mainFeatureId);
        }
        public ProductMainFeature GetProductMainFeature(int productId)
        {
            return _context.ProductMainFeatures.Include(f => f.Feature).Include(f => f.SubFeature).Where(f => f.IsDeleted == false && f.ProductId == productId).FirstOrDefault();
        }
        public void UpdateQuantity(int id,int newquantity)
        {
            var oldquantity= _context.ProductMainFeatures.Where(a => a.Id == id).FirstOrDefault();
            oldquantity.Quantity = oldquantity.Quantity - newquantity;
            _context.Entry(oldquantity).State = EntityState.Modified;
            _context.SaveChanges();
            _logger.LogEvent(oldquantity.GetType().Name, oldquantity.Id, "Update");


        }
        public List<ProductMainFeature> productMainFeatures(int SubFeatuerId, int ProductGroupId)
        {
           var productsubfeature= _context.ProductMainFeatures.Where(a => a.SubFeatureId == SubFeatuerId).Include(a=>a.Product).ToList();
            var qq = productsubfeature.Where(a => a.Product.ProductGroupId == ProductGroupId).ToList();
            return qq;
        }
    }
}
