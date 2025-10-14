using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PayStack.Net;
using System.Security.Claims;
using Tryout.DataAccess.Repository.IRepository;
using Tryout.Models;
using Tryout.Models.ViewModels;
using Tryout.Utility;

namespace Tryout.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaystackSetting _paystackSetting;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IOptions<PaystackSetting> paystackSetting)
        {
            _unitOfWork=unitOfWork;
            _paystackSetting = paystackSetting.Value;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };
            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u=>u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
      
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u=>u.Id == userId);

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            if(applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //Regular Customer
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //Company User
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach(var cart in ShoppingCartVM.ShoppingCartList) {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            if(applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //If Its A Regular Customer


                // === PAYSTACK Initialization ===

                // Use PayStack.Net wrapper
                var paystack = new PayStackApi(_paystackSetting.SecretKey);

                // Paystack expects amount in kobo (if currency is NGN). Adjust as per your currency:
                // Here we assume NGN: multiply by 100 to convert naira -> kobo
                var amountInKobo = (int)Math.Round(ShoppingCartVM.OrderHeader.OrderTotal * 100);

                var reference = $"ORD_{ShoppingCartVM.OrderHeader.Id}_{DateTime.Now.Ticks}";
                
                var domain = $"{Request.Scheme}://{Request.Host.Value}/";

                var initializeRequest = new TransactionInitializeRequest
                {
                    AmountInKobo = amountInKobo,
                    Email = applicationUser.Email,
                    Reference = reference,
                    CallbackUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}"
                };

                var initializeResponse = paystack.Transactions.Initialize(initializeRequest);

                if (initializeResponse.Status && initializeResponse.Data != null)
                {
                    // Save Paystack reference in DB so we can verify later
                    _unitOfWork.OrderHeader.UpdatePaymentReference(ShoppingCartVM.OrderHeader.Id, initializeResponse.Data.Reference);
                    _unitOfWork.Save();

                    // Redirect customer to Paystack payment page
                    Response.Headers.Add("Location", initializeResponse.Data.AuthorizationUrl);
                    return new StatusCodeResult(303);
                }
                else
                {
                    // handle failed init
                    TempData["error"] = "Payment initialization failed. Please try again.";
                    return RedirectToAction(nameof(Summary));
                }

            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            // load order header
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");

            if (orderHeader == null)
                return NotFound();

            // If order was delayed/company we already approved earlier
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                // clear cart, send email etc, same as before
                // Clear session and cart items
                var shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
                _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
                _unitOfWork.Save();

                return View(id);
            }

            // Verify payment using Paystack (use saved PaymentReference)
            var paystack = new PayStackApi(_paystackSetting.SecretKey);
            var reference = orderHeader.PaymentReference;
            if (string.IsNullOrEmpty(reference))
            {
                // nothing to verify
                TempData["error"] = "Payment reference is missing.";
                return View(id);
            }

            var verifyResponse = paystack.Transactions.Verify(reference);
            if (verifyResponse.Status && verifyResponse.Data.Status == "success")
            {
                // mark paid
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();

                // clear cart items and session
                var shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
                _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
                _unitOfWork.Save();

                // optionally send email
                // _emailSender.SendEmailAsync(...);

                return View(id);
            }
            else
            {
                // payment failed / pending
                TempData["error"] = "Payment verification failed or pending.";
                return View(id);
            }
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.Id==cartId);
            cartFromDb.Count+=1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.Id==cartId);
            if (cartFromDb.Count<=1)
            {
                //Remove Item From Cart
                _unitOfWork.ShoppingCart.Remove(cartFromDb);

            }
            else
            {
                cartFromDb.Count-=1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);

            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.Id==cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            } else
            {
                if(shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                } else return shoppingCart.Product.Price100;
            }
        }
    }
}
