namespace STech.Data.ViewModels.VNPay
{
    public class PaymentInformationModel
    {
        public string OrderId { get; set; } = null!;
        public string OrderType { get; set; } = null!;
        public double Amount { get; set; }
        public string OrderDescription { get; set; } = null!;
        public string Name { get; set; } = null!;

    }
}
