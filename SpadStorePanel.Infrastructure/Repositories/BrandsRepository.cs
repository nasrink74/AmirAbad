using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpadStorePanel.Core.Models;

namespace SpadStorePanel.Infrastructure.Repositories
{
    public class BrandsRepository : BaseRepository<Brand, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public BrandsRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<Brand> brands(int? ProductGroupId)
        {
            var brandid = _context.ProductGroupBrands.Where(a => a.ProductGroupId == ProductGroupId &a.IsDeleted==false).ToList();
            var brandsname = new List<Brand>();
            foreach(var item in brandid)
            {
                var bb=_context.Brands.Where(a => a.Id == item.BrandId).FirstOrDefault();
                brandsname.Add(bb);
            }
            return brandsname;
        }

        public List<Brand> GetAllGroupBrands(int groupId)
        {
            var pgBrands = _context.ProductGroupBrands.Where(f => f.ProductGroupId == groupId).ToList();
            var brands = pgBrands.Select(item => _context.Brands.Find(item.BrandId)).ToList();
            return brands;
        }
    }
}
