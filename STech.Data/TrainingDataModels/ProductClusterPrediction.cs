namespace STech.Data.TrainingDataModels
{
    public class ProductClusterPrediction
    {
        public uint PredictedLabel { get; set; }
        public string ProductId { get; set; } = null!;
        public float Price { get; set; }
    }
}
