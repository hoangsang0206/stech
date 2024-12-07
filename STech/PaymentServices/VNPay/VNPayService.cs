using STech.Data.ViewModels.VNPay;
using STech.PaymentModels.VNPay;

namespace STech.PaymentServices.VNPay
{
    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _configuration;

        private readonly IConfigurationSection _section;

        public VNPayService(IConfiguration configuration)
        {
            _configuration = configuration;
            _section = _configuration.GetSection("Payments");
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_section["TimeZoneId"] ?? "");
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VNPayModel();
            var urlCallBack = _section["Vnpay:PaymentBackReturnUrl"];

            pay.AddRequestData("vnp_Version", _section["Vnpay:Version"] ?? "");
            pay.AddRequestData("vnp_Command", _section["Vnpay:Command"] ?? "");
            pay.AddRequestData("vnp_TmnCode", _section["Vnpay:TmnCode"] ?? "");
            pay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _section["Vnpay:CurrCode"] ?? "");
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _section["Vnpay:Locale"] ?? "");
            pay.AddRequestData("vnp_OrderInfo", model.OrderDescription);
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack ?? "");
            pay.AddRequestData("vnp_ExpireDate", timeNow.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_TxnRef", $"{model.OrderId}{tick}");

            var paymentUrl =
                pay.CreateRequestUrl(_section["Vnpay:BaseUrl"] ?? "", _section["Vnpay:HashSecret"] ?? "");

            return paymentUrl;
        }


        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VNPayModel();
            var response = pay.GetFullResponseData(collections, _section["Vnpay:HashSecret"] ?? "");

            return response;
        }

    }
}
