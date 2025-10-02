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


        public void UpdateStatus(int Id, string orderStatus, string paymentStatus)
        {
            var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(o => o.Id == Id);
            if (orderHeaderFromDb != null)
            {
                orderHeaderFromDb.OrderStatus = orderStatus;

                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderHeaderFromDb.PaymentStatus = paymentStatus;

                }
                //_db.OrderHeaders.Update(orderHeader);
            }
        }

        public void UpdatePaymentReference(int Id, string reference)
        {
            var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(o => o.Id == Id);
            if (orderHeaderFromDb != null)
            {
                if (!string.IsNullOrEmpty(reference))
                {
                    orderHeaderFromDb.PaymentReference = reference;
                    orderHeaderFromDb.PaymentDate = DateTime.Now;

                }
            }
        }

    }
}
