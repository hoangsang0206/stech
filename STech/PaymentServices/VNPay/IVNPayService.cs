using STech.Data.ViewModels.VNPay;

namespace STech.PaymentServices.VNPay
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
