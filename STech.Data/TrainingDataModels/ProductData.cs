namespace STech.Data.TrainingDataModels
{
    public class ProductData
    {
        public string ProductId { get; set; } = null!;

        public float Price { get; set; }

        public int Warranty { get; set; }

        public string CategoryName { get; set; } = null!;

        public string BrandName { get; set; } = null!;

        public string Specifications { get; set; } = null!;
    }
}
