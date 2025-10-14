using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tryout.Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
        //public string Status { get; set; }
        //public string PaymentStatus { get; set; }
        //public string PaymentIntentId { get; set; }
        //public string SessionId { get; set; }
        //public string StripePublicKey { get; set; }
    }
}
