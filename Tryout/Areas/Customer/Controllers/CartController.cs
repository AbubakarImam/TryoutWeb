using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaystackSetting _paystackSetting;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, IOptions<PaystackSetting> paystackSetting)
        {
            _unitOfWork=unitOfWork;
            _paystackSetting = paystackSetting.Value;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
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
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

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

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
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

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
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
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
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

        public async Task<IActionResult> OrderConfirmation(int id)
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
            HttpContext.Session.Clear();


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

                // ✅ Send confirmation email
                string subject = $"Your Order #{orderHeader.Id} has been placed successfully!";
                string body = $@"
<div style='text-align:center;'>
        <img src='https://res.cloudinary.com/dzl44lobc/image/upload/v1761965012/kamshiLogo_nxgi0y.svg' alt='Kamshi Store Logo' style='width:150px; height:auto;' />
    </div>
            <h2>Thank you for shopping with Kamshi Store!</h2>
            <p>Hi {orderHeader.ApplicationUser.Name},</p>
            <p>Your order <strong>#{orderHeader.Id}</strong> has been confirmed and is now being processed.</p>
            <p><strong>Order Summary:</strong></p>
            <ul>
                {string.Join("", _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == id, includeProperties: "Product")
                            .Select(d => $"<li>{d.Product?.Title ?? "Unknown"} - {d.Count} x {d.Price:C}</li>"))}
            </ul>
            <p><strong>Total:</strong> {orderHeader.OrderTotal:C}</p>
            <p>We'll send you another email once your order ships.</p>
            <br/>
            <p>❤️ Kamshi Store</p>
        ";

                await _emailSender.SendEmailAsync(subject, orderHeader.ApplicationUser.Email, body);


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
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id==cartId);
            cartFromDb.Count+=1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id==cartId, tracked: true);
            if (cartFromDb.Count<=1)
            {
                //Remove Item From Cart
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll
                (u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count()-1);
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
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id==cartId, tracked: true);
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll
                (u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count()-1);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            return shoppingCart.UnitType.ToLower() switch
            {
                "6ml" => shoppingCart.Product.Price6ml,
                "10ml" => shoppingCart.Product.Price10ml,
                "15ml" => shoppingCart.Product.Price15ml,
                "20ml" => shoppingCart.Product.Price20ml,
                _ => shoppingCart.Product.Price6ml // fallback
            };
        }
    }
}
