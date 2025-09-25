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
    public class OrderHeaderRepository : Repository<OrderHeader>,IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }
    }
}
