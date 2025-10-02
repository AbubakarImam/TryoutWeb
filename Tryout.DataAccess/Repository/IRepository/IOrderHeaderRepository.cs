using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tryout.Models;

namespace Tryout.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
        void UpdatePaymentReference(int Id, string reference);
        void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null);
    }
}
