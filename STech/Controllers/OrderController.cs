using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Services.Constants;
using STech.Services.Services;
using STech.Utils;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text.Json;

namespace STech.Controllers
{
    public class OrderController : Controller
    {
        private readonly AddressService _addressService;
        private readonly IGeocodioService _geocodioService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IPaymentService _paymentService;
        private readonly IWarehouseService _warehouseService;

        private readonly HttpClient _httpClient;

        public OrderController(AddressService addressService, IGeocodioService geocodioService,
            IUserService userService, IProductService productService, IOrderService orderService,
            ICartService cartService, IPaymentService paymentService, IWarehouseService warehouseService,
            HttpClient httpClient)
        {
            _addressService = addressService;
            _geocodioService = geocodioService;
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _cartService = cartService;
            _paymentService = paymentService;
            _warehouseService = warehouseService;

            _httpClient = httpClient;
        }

        private string GetDomain()
        {
            HttpRequest request = HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }

        private AddressVM GetAddress(string? wardCode, string? districtCode, string? cityCode)
        {
            AddressVM address = new AddressVM();
            address._City = _addressService.Address.Cities.FirstOrDefault(c => c.code == cityCode);
            address._District = address._City?.districts.FirstOrDefault(c => c.code == districtCode);
            address._Ward = address._District?.wards.FirstOrDefault(c => c.code == wardCode);

            return address;
        }

        private async Task<decimal> CalculateShippingFee(AddressVM address_1, AddressVM address_2)
        {
            if(address_1._Ward == null || address_1._District == null || address_1._City == null 
                || address_2._Ward == null || address_2._District == null || address_2._City == null )
            {
                return 0;
            }

            var (latitude, longtitude) = await _geocodioService
                .GetLocation(address_1._City.name_with_type, address_1._District.name_with_type, address_1._Ward.name_with_type);
            var (whLat, whLong) = await _geocodioService
                .GetLocation(address_2._City.name_with_type, address_2._District.name_with_type, address_2._Ward.name_with_type);

            if (!latitude.HasValue || !longtitude.HasValue || !whLat.HasValue || !whLong.HasValue)
            {
                return 0;
            }

            double distance = GeocodioUtils.CalculateDistance(latitude.Value, longtitude.Value, whLat.Value, whLong.Value);
            double fee = GeocodioUtils.CalculateFee(distance);

            return Convert.ToDecimal(fee);
        }

        private Data.Models.Invoice CreateInvoice(OrderVM order, Data.Models.PaymentMethod paymentMethod, AddressVM address, string userId)
        {
            DateTime date = DateTime.Now;
            Data.Models.Invoice invoice = new Data.Models.Invoice();
            invoice.InvoiceId = date.ToString("yyyyMMdd") + RandomUtils.GenerateRandomString(8).ToUpper();
            invoice.OrderDate = date;
            invoice.PaymentMedId = paymentMethod.PaymentMedId;
            invoice.PaymentStatus = PaymentContants.UnPaid;
            invoice.DeliveryMedId = order.DeliveryMethod;
            invoice.UserId = userId;
            invoice.Note = order.Note;
            invoice.DeliveryAddress = $"{order.Address}, {address._Ward?.name_with_type}, {address._District?.name_with_type}, {address._City?.name_with_type}";
            invoice.RecipientName = order.RecipientName;
            invoice.RecipientPhone = order.RecipientPhone;
            invoice.IsCompleted = false;

            return invoice;
        }

        private async Task<PackingSlip> CreatePackingSlip(Data.Models.Invoice invoice, AddressVM address)
        {
            Warehouse warehouse = await _warehouseService.GetOnlineWarehouse() ?? new Warehouse();
            AddressVM whAddress = GetAddress(warehouse.WardCode, warehouse.DistrictCode, warehouse.ProvinceCode);

            PackingSlip packingSlip = new PackingSlip();
            packingSlip.InvoiceId = invoice.InvoiceId;
            packingSlip.Psid = DateTime.Now.ToString("yyyyMMdd") + "STECH" + RandomUtils.GenerateRandomString(8).ToUpper();
            packingSlip.DeliveryFee = await CalculateShippingFee(address, whAddress);
            packingSlip.IsCompleted = false;

            return packingSlip;
        }

        private List<InvoiceStatus> CreateInvoiceStatus(Data.Models.Invoice invoice)
        {
            List<InvoiceStatus> invoiceStatuses = new List<InvoiceStatus>
            {
                new InvoiceStatus
                {
                    InvoiceId = invoice.InvoiceId,
                    Status = "Chờ xác nhận",
                    DateUpdated = invoice.OrderDate
                }
            };

            return invoiceStatuses;
        }

        private async Task<List<InvoiceDetail>> CreateInvoiceDetails(Data.Models.Invoice invoice, string? pId)
        {
            List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();

            if (pId != null)
            {
                Data.Models.Product? product = await _productService.GetProduct(pId);
                if (product != null)
                {
                    invoiceDetails.Add(new InvoiceDetail
                    {
                        InvoiceId = invoice.InvoiceId,
                        ProductId = product.ProductId,
                        Quantity = 1,
                        Cost = product.Price
                    });
                }
            }
            else
            {
                if(invoice.UserId != null)
                {
                    IEnumerable<UserCart> userCart = await _cartService.GetUserCart(invoice.UserId);
                    foreach (UserCart cart in userCart)
                    {
                        invoiceDetails.Add(new InvoiceDetail
                        {
                            InvoiceId = invoice.InvoiceId,
                            ProductId = cart.Product.ProductId,
                            Quantity = cart.Quantity,
                            Cost = cart.Product.Price
                        });
                    }
                }

            }

            return invoiceDetails;
        }

        private Data.Models.Invoice? GetInvoiceFromSession()
        {
            string invoiceJson = HttpContext.Session.GetString("Invoice") ?? "";
            HttpContext.Session.Remove("Invoice");
            Data.Models.Invoice? invoice = JsonSerializer.Deserialize<Data.Models.Invoice>(invoiceJson);

            return invoice;
        }

        [Authorize]
        public async Task<IActionResult> CheckOut(string? pId = null, string? ward = null, string? district = null, string? city = null)
        {
            string? userId = User.FindFirstValue("Id");
            if(userId == null)
            {
                return BadRequest();
            }

            User? user = await _userService.GetUserById(userId);
            if(user == null)
            {
                return BadRequest();
            }

            IEnumerable<UserAddress> userAddresses = await _userService.GetUserAddress(userId);
            user.UserAddresses = userAddresses as List<UserAddress> ?? new List<UserAddress>();
            UserAddress? defaultAddress = userAddresses.FirstOrDefault(t => t.IsDefault == true);

            AddressVM address = new AddressVM();
            city ??= defaultAddress?.ProvinceCode;
            district ??= defaultAddress?.DistrictCode;
            ward ??= defaultAddress?.WardCode;

            address = GetAddress(ward, district, city);

            Data.Models.Product? product = null; 

            if (pId == null)
            {
                IEnumerable<UserCart> cart = await _cartService.GetUserCart(userId);

                List<UserCart> cartToDelete = new List<UserCart>();
                foreach (UserCart c in cart)
                {
                    int qty = await _productService.GetTotalQty(c.ProductId);
                    if (qty <= 0)
                    {
                        cartToDelete.Add(c);
                    }
                    else if (c.Quantity > qty)
                    {
                        await _cartService.UpdateQuantity(c, qty);
                    }
                }

                if (cartToDelete.Count > 0)
                {
                    await _cartService.RemoveListCart(cartToDelete);
                }

                cart = await _cartService.GetUserCart(userId);

                user.UserCarts = cart as List<UserCart> ?? new List<UserCart>();

                if(cart.Count() <= 0)
                {
                    return LocalRedirect("/");
                }
            }
            else
            {
                product = await _productService.GetProductWithBasicInfo(pId);

                if(product == null)
                {
                    return LocalRedirect("/");
                }

                if (await _productService.CheckOutOfStock(pId))
                {
                    return LocalRedirect("/");
                }
            }

            return View(new Tuple<User, AddressVM, Data.Models.Product?>(user, address, product));
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderVM order)
        {
            if(ModelState.IsValid)
            {
                string? userId = User.FindFirstValue("Id");
                if(userId == null)
                {
                    return BadRequest();
                }

                User? user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return BadRequest();
                }

                Data.Models.PaymentMethod? paymentMethod = await _paymentService.GetPaymentMethod(order.PaymentMethod);
                if(paymentMethod == null)
                {
                    return BadRequest();
                }

                AddressVM address = GetAddress(order.WardCode, order.DistrictCode, order.CityCode);

                Data.Models.Invoice invoice = CreateInvoice(order, paymentMethod, address, userId);
                invoice.PackingSlip = await CreatePackingSlip(invoice, address);
                invoice.InvoiceStatuses = CreateInvoiceStatus(invoice);
                invoice.InvoiceDetails = await CreateInvoiceDetails(invoice, order.pId);

                invoice.SubTotal = invoice.InvoiceDetails.Sum(t => t.Cost * t.Quantity);
                invoice.Total = invoice.SubTotal + invoice.PackingSlip.DeliveryFee;

                switch(order.PaymentMethod)
                {
                    case PaymentContants.CashPayment:
                        bool result = await _orderService.CreateInvoice(invoice);
                        if(result)
                        {
                            return RedirectToAction("PaymentSucceeded");
                        }
                        else
                        {
                            return RedirectToAction("PaymentFailed");
                        }

                    case PaymentContants.CardPayment:
                        HttpContext.Session.SetString("Invoice", JsonSerializer.Serialize(invoice));
                        return RedirectToAction("PaymentWithStripe");
                            
                    case PaymentContants.PaypalPayment:
                        HttpContext.Session.SetString("Invoice", JsonSerializer.Serialize(invoice));
                        return RedirectToAction("PaymentWithPaypal");

                    default:
                        return BadRequest();
                }
            }

            return BadRequest();
        }

        [Authorize]
        public async Task<IActionResult> PaymentWithStripe()
        {
            Data.Models.Invoice? invoice = GetInvoiceFromSession();

            if(invoice != null)
            {
                List<SessionLineItemOptions> items = new List<SessionLineItemOptions>();
                foreach(InvoiceDetail detail in invoice.InvoiceDetails)
                {
                    Data.Models.Product? product = await _productService.GetProductWithBasicInfo(detail.ProductId);

                    items.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)Math.Round((detail.Cost) / PaymentContants.USD_EXCHANGE_RATE) * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = product?.ProductName,
                                Images = new List<string> { product?.ProductImages.FirstOrDefault()?.ImageSrc ?? "" }
                            }
                        },
                        Quantity = detail.Quantity
                    });
                }

                if (invoice.PackingSlip != null)
                {
                    items.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)Math.Round((invoice.PackingSlip.DeliveryFee) / PaymentContants.USD_EXCHANGE_RATE) * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Delivery Fee"
                            }
                        }
                    });
                }

                SessionCreateOptions options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = items,
                    Metadata = new Dictionary<string, string>
                    {
                        { "invoice_id", invoice.InvoiceId }
                    },
                    Mode = "payment",
                    SuccessUrl = $"{GetDomain()}/order/stripecallback",
                    CancelUrl = $"{GetDomain()}/order/paymentfailed"
                };
                
                SessionService service = new SessionService();
                Session session = service.Create(options);
                TempData["Invoice"] = session.Id;

                if(await _orderService.CreateInvoice(invoice)) {
                    return Redirect(session.Url);
                }
                else
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> StripeCallback()
        {
            SessionService service = new SessionService();
            Session session = service.Get(TempData["Invoice"]?.ToString());
            string invoiceId = session.Metadata["invoice_id"];
            Data.Models.Invoice? invoice = await _orderService.GetInvoice(invoiceId);

            if(invoice == null)
            {
                return RedirectToAction("PaymentFailed");
            }
            
            if(session.PaymentStatus == "paid")
            {
                invoice.PaymentStatus = PaymentContants.Paid;
                await _orderService.UpdateInvoice(invoice);

                return RedirectToAction("PaymentSucceeded");
            }

            invoice.PaymentStatus = PaymentContants.PaymentFailed;
            await _orderService.UpdateInvoice(invoice);
            return RedirectToAction("PaymentFailed");
        }

        [Authorize]
        public async Task<IActionResult> PaymentWithPaypal(Data.Models.Invoice invoice)
        {
            return Ok();
        }

        [Authorize]
        public IActionResult PaymentSucceeded()
        {

            return View();
        }

        [Authorize]
        public IActionResult PaymentFailed()
        {
            return View();
        }

        public IActionResult CheckOrder(string oId, string phone)
        {
            return View();
        }
    }
}
