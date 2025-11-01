using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using PayStack.Net;
using System.Diagnostics;
using System.Security.Claims;
using Tryout.DataAccess.Repository.IRepository;
using Tryout.Models;
using Tryout.Models.ViewModels;
using Tryout.Utility;


namespace Tryout.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaystackSetting _paystackSetting;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork, IEmailSender emailSender, IOptions<PaystackSetting> paystackSetting)
        {
            _unitOfWork = unitOfWork;
            _paystackSetting = paystackSetting.Value;
            _emailSender = emailSender;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully.";


            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> ShipOrder()
        {

            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            string subject = $"Your Order #{orderHeader.Id} has been shipped!";
            string body = $@"
 <div style='text-align:center;'>
        <img src='https://res.cloudinary.com/dzl44lobc/image/upload/v1761965012/kamshiLogo_nxgi0y.svg' alt='Kamshi Store Logo' style='width:150px; height:auto;' />
    </div>
        <h2>Your order is on the way 🚚</h2>
        <p>Hi {orderHeader.ApplicationUser.Name},</p>
        <p>Your order <strong>#{orderHeader.Id}</strong> has been shipped and will arrive soon.</p>
        <p>Thank you for shopping with Kamshi Store!</p>
    ";

            await _emailSender.SendEmailAsync(subject, orderHeader.ApplicationUser.Email, body); 
            TempData["Success"] = "Order Shipped Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            if (orderHeader == null)
            {
                TempData["error"] = "Order not found.";
                return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
            }

            // Only refund if payment was made
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _paystackSetting.SecretKey);

                        var refundData = new Dictionary<string, string>
                {
                    { "transaction", orderHeader.PaymentReference }, // your stored Paystack ref
                    { "amount", ((int)(orderHeader.OrderTotal * 100)).ToString() } // amount in kobo
                };

                        var content = new FormUrlEncodedContent(refundData);
                        var response = await client.PostAsync("https://api.paystack.co/refund", content);
                        var json = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            // Optional: parse JSON for refund confirmation
                            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
                            TempData["success"] = "Order cancelled and refund processed successfully.";
                        }
                        else
                        {
                            TempData["error"] = $"Refund failed. Details: {json}";
                            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.PaymentStatusPending);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"Refund error: {ex.Message}";
                }
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
                TempData["success"] = "Order cancelled successfully (no payment to refund).";
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }



        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_PAY_NOW()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeader
                .Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetail
                .GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

            // Total amount in naira
            double totalAmount = OrderVM.OrderDetail.Sum(x => x.Price * x.Count);

            var applicationUser = OrderVM.OrderHeader.ApplicationUser;

            // === PAYSTACK Initialization ===
            var paystack = new PayStack.Net.PayStackApi(_paystackSetting.SecretKey);
            var reference = $"ORD_{OrderVM.OrderHeader.Id}_{DateTime.Now.Ticks}";
            var domain = $"{Request.Scheme}://{Request.Host.Value}/";

            var initializeRequest = new PayStack.Net.TransactionInitializeRequest
            {
                AmountInKobo = (int)Math.Round(totalAmount * 100), // convert to kobo
                Email = applicationUser.Email,
                Reference = reference,
                CallbackUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}"
            };

            var initializeResponse = paystack.Transactions.Initialize(initializeRequest);

            if (initializeResponse.Status && initializeResponse.Data != null)
            {
                // Save Paystack reference in DB
                _unitOfWork.OrderHeader.UpdatePaymentReference(OrderVM.OrderHeader.Id, initializeResponse.Data.Reference);
                _unitOfWork.Save();

                // Redirect to Paystack checkout
                Response.Headers.Add("Location", initializeResponse.Data.AuthorizationUrl);
                return new StatusCodeResult(303);
            }
            else
            {
                TempData["error"] = "Payment initialization failed. Please try again.";
                return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
            }
        }


        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId, includeProperties: "ApplicationUser");
            if (orderHeader == null)
                return NotFound();

            // Verify payment with Paystack
            var paystack = new PayStack.Net.PayStackApi(_paystackSetting.SecretKey);
            var reference = orderHeader.PaymentReference;

            if (string.IsNullOrEmpty(reference))
            {
                TempData["error"] = "Payment reference is missing.";
                return View(orderHeaderId);
            }

            var verifyResponse = paystack.Transactions.Verify(reference);

            if (verifyResponse.Status && verifyResponse.Data.Status == "success")
            {
                // mark as paid
                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                _unitOfWork.Save();

                TempData["success"] = "Payment verified successfully!";
            }
            else
            {
                TempData["error"] = "Payment verification failed or pending.";
            }

            return View(orderHeaderId);
        }




        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders;


            if (User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeaders = _unitOfWork.OrderHeader
                    .GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }


            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;

            }


            return Json(new { data = objOrderHeaders });
        }


        #endregion
    }
}