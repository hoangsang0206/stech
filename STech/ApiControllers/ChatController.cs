using Microsoft.AspNetCore.Mvc;
using STech.Chatbot;
using STech.Chatbot.Models;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Utils;
using System.Security.Claims;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;
        private readonly string[] _intentList;
        private readonly string _defaultMessage;

        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        private readonly string _policiesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DataFiles", "Policies");

        public ChatController(IChatbotService chatbotService,
            IProductService productService,
            IOrderService orderService)
        {
            _chatbotService = chatbotService;
            _intentList = new string[] {
                Intents.FindProduct,
                Intents.TrackOrder,
                Intents.CurrentOrder,
                Intents.FindProductByBrand,
                Intents.FindProductByCategory,
                Intents.PaymentPolicy,
                Intents.DeliveringPolicy
            };
            _defaultMessage = "Tôi không hiểu câu hỏi của bạn, " +
                "bạn có thể hỏi các câu liên quan đến đơn hàng hoặc sản phẩm.";

            _productService = productService;
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatbotRequest request)
        {
            string message = "";
            bool isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            string? userId = isAuthenticated ? User.FindFirstValue("Id") : null;

            var parseResponse = await _chatbotService.GetParseResponse(request.Message.ToLower());

            string? intent = parseResponse?.intent.name;

            if (intent == null || !_intentList.Contains(intent))
            {
                var messageResponse = await _chatbotService.GetMessageResponse(request.Message.ToLower());
                message = GenerateMessageHTML(messageResponse?.text ?? _defaultMessage);
            }
            else
            {
                switch (intent)
                {
                    case Intents.CurrentOrder:
                    {
                        if (!isAuthenticated)
                        {
                            message = GenerateMessageHTML("Bạn cần đăng nhập để xem đơn hàng hiện tại.");
                            break;
                        }

                        Invoice? inv = await _orderService.GetUserCurrentOrder(userId ?? "");
                        if (inv == null)
                        {
                                message = GenerateMessageHTML("Bạn chưa có đơn hàng nào hết. " +
                                    "Hãy đặt ngay cho mình 1 đơn hàng nào !!!");
                        }
                        else
                        {
                            message = GenerateOrderHTML(new List<Invoice> { inv }, "Dưới đây là đơn hàng gần nhất của bạn: ");
                        }

                        break;
                    }

                    case Intents.TrackOrder:
                    {
                        if (!isAuthenticated)
                        {
                            message = GenerateMessageHTML("Bạn cần đăng nhập để xem đơn hàng.");
                            break;
                        }

                        string? invoiceId = parseResponse?.entities
                                .FirstOrDefault(e => e.entity == Entities.OrderCode)?.value;

                        if (invoiceId == null)
                        {
                            message = GenerateMessageHTML("Bạn có thể cung cấp mã đơn hàng trong câu hỏi để tôi có thể tìm giúp bạn.");
                            break;
                        }

                        Invoice? inv = await _orderService.GetUserInvoice(invoiceId, userId ?? "");
                        if (inv == null)
                        {
                            message = GenerateMessageHTML("Có vẻ như bạn đã đưa tôi sai mã đơn hàng. Hãy kiểm tra lại một lần nữa");
                        }
                        else
                        {
                            message = GenerateOrderHTML(new List<Invoice> { inv }, "Dưới đây là đơn hàng của bạn: ");
                        }

                        break;
                    }

                    case Intents.FindProduct:
                    {
                        var (specs, productId, productName, priceStr) = GetProductEntities(parseResponse);
                        var products = await _productService.SearchProductsByChatBotData(specs, productId, productName, priceStr);

                        if(!products.Any())
                        {
                            message = GenerateMessageHTML("Tôi không tìm thấy sản phẩm nào phù hợp với yêu cầu của bạn.");
                        }
                        else
                        {
                            message = GenerateProductHTML(products.ToList());
                        }

                        break;
                    }

                    case Intents.FindProductByBrand:
                    {
                        string? brand = parseResponse?.entities
                                .FirstOrDefault(e => e.entity == Entities.Brand)?.value;
                        if(brand == null)
                        {
                            message = GenerateMessageHTML("Tôi không tìm thấy sản phẩm nào phù hợp với yêu cầu của bạn.");
                            break;
                        }
                        var products = await _productService.SearchProductsByBrandName(brand);
                        message = GenerateProductHTML(products.ToList());
                        break;
                    }

                    case Intents.FindProductByCategory:
                    {
                        string? category = parseResponse?.entities
                                .FirstOrDefault(e => e.entity == Entities.Category)?.value;
                        if (category == null)
                        {
                            message = GenerateMessageHTML("Tôi không tìm thấy sản phẩm nào phù hợp với yêu cầu của bạn.");
                            break;
                        }
                        var products = await _productService.SearchProductsByCategoryName(category);
                        message = GenerateProductHTML(products.ToList());
                        break;
                    }

                    case Intents.PaymentPolicy:
                        message = GeneratePaymentPolicyHTML();
                        break;

                    case Intents.DeliveringPolicy:
                        message = GenerateDeliveringPolicyHTML();
                        break;


                    default:
                        message = GenerateMessageHTML(_defaultMessage);
                        break;
                }
            }

            

            return Ok(new ChatbotResponse
            {
                Message = message
            });
        }

        private (List<ProductSpecification>?, string?, string?, string?) GetProductEntities(ChatbotParseResponse? parseResponse)
        {
            if(parseResponse == null)
            {
                return (null, null, null, null);
            }

            var specNames = parseResponse.entities.Where(e => e.entity == Entities.SpecName).Select(e => e.value).ToList();
            var specValues = parseResponse.entities.Where(e => e.entity == Entities.SpecValue).Select(e => e.value).ToList();
            var specs = specNames.Zip(specValues, (name, value) => new ProductSpecification
            {
                SpecName = name,
                SpecValue = value
            }).ToList();

            string? productId = parseResponse.entities.FirstOrDefault(e => e.entity == Entities.ProductCode)?.value;
            string? productName = parseResponse.entities.FirstOrDefault(e => e.entity == Entities.ProductName)?.value;
            string? price = parseResponse.entities.FirstOrDefault(e => e.entity == Entities.Price)?.value;

            return (specs, productId, productName, price);
        }

        private string GenerateMessageHTML(string message)
        {
            return $"<div class=\"chatbot-message-text\"><span>{message}</span></div>";
        }

        private string GenerateOrderHTML(List<Invoice> orders, string message)
        {
            string baseMessage = $"<div class=\"chatbot-message-text\"><span>{message}</span></div>";
            string orderHTML = "<div class=\"chat-item-list\">";

            foreach (var order in orders)
            {
                orderHTML +=
                        $"<a href=\"/order/detail/{order.InvoiceId}\" target=\"_blank\" class=\"chat-order d-flex gap-2\">" +
                            "<div class=\"chat-order-icon\">" +
                                "<i class=\"fa-solid fa-cart-flatbed\"></i>" +
                            "</div>" +
                            "<div class=\"chat-order-info\">" +
                                $"<div>Mã ĐH: <span class=\"chat-info-bold\">{order.InvoiceId}</span></div>" +
                                $"<div>Ngày đặt: <span class=\"chat-info-bold\">{order.OrderDate?.ToString("dd/MM/yyyy HH:mm")}</span></div>" +
                                $"<div>Trạng thái: {GenerateOrderStatusHTML(order)}</div>" +
                            "</div>" +
                        "</a>";
            }

            return baseMessage + orderHTML + "</div>";
        }

        private string GenerateOrderStatusHTML(Invoice invoice)
        {
            string orderStatus = "";

            if (invoice.IsCancelled)
            {
                orderStatus = "<span class=\"chat-info-bold\" style=\"color: #e30019\">Đã hủy</span>";
            }
            else if (invoice.IsCompleted)
            {
                orderStatus = "<span class=\"chat-info-bold\" style=\"color: #37b302\">Đã giao hàng</span>";
            }
            else if (invoice.IsAccepted)
            {
                orderStatus = "<span class=\"chat-info-bold\" style=\"color: #ff9b10\">Chờ giao hàng</span>";
            }
            else
            {
                orderStatus = "<span class=\"chat-info-bold\" style=\"color: #ff9b10\">Chờ xác nhận</span>";
            }

            return orderStatus;
        }

        private string GenerateProductHTML(List<Product> products)
        {
            string baseMessage = $"<div class=\"chatbot-message-text\"><span>" +
                $"Dưới đây là danh sách sản phẩm phù hợp với yêu cầu của bạn:</span></div>";
            string orderHTML = "<div class=\"chat-item-list\">";

            foreach (var product in products)
            {
                string imgSrc = product.ProductImages?.FirstOrDefault()?.ImageSrc ?? "/images/no-image.jpg";

                orderHTML +=
                        $"<a href=\"product/{product.ProductId}\" target=\"_blank\" class=\"chat-product d-flex gap-2\">" +
                            "<div class=\"chat-product-image\">" +
                                $"<img src=\"{imgSrc}\" alt=\"\" />" +
                            "</div>" +
                            "<div class=\"chat-product-info\">" +
                                $"<div>Mã sản phẩm: <span class=\"chat-info-bold\">{product.ProductId}</span></div>" +
                                $"<div>Tên sản phẩm: <span class=\"chat-info-bold\">{product.ProductName}</span></div>" +
                                $"<div>Giá bán:  <span class=\"chat-info-bold\">{CurrencyFormatter.Format(product.Price)}</span></div>" +
                            "</div>" +
                        "</a>";
            }

            return baseMessage + orderHTML + "</div>";
        }

        private string GeneratePaymentPolicyHTML()
        {
            try
            {
                string filePath = Path.Combine(_policiesFilePath, "payment.html");
                return "<div class=\"chatbot-message-text\">" + System.IO.File.ReadAllText(filePath) + "</div>";
            }
            catch (Exception ex)
            {
                return GenerateMessageHTML("Tôi không tìm thấy chính sách thanh toán");
            }
        }

        private string GenerateDeliveringPolicyHTML()
        {
            try
            {
                string filePath = Path.Combine(_policiesFilePath, "delivery.html");
                return "<div class=\"chatbot-message-text\">" + System.IO.File.ReadAllText(filePath) + "</div>";
            }
            catch (Exception ex)
            {
                return GenerateMessageHTML("Tôi không tìm thấy chính sách giao hàng");
            }
        }
    }
}
