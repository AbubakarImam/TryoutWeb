using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tryout.DataAccess.Data;
using Tryout.DataAccess.Repository.IRepository;
using Tryout.Models;

namespace Tryout.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            //_db.Products.Update(obj);
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.InspirationBrand = obj.InspirationBrand;
                objFromDb.SKU = obj.SKU;
                objFromDb.Price6ml = obj.Price6ml;
                objFromDb.Price10ml = obj.Price10ml;
                objFromDb.Price15ml = obj.Price15ml;
                objFromDb.Price20ml = obj.Price20ml;             
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.ProductImages = obj.ProductImages; objFromDb.Title = obj.Title;
                
            }
        }
    }
}
