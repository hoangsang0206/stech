using Microsoft.ML;
using Microsoft.ML.Data;
using STech.Data.TrainingDataModels;

namespace STech.TrainModel
{
    public static class KMeans
    {
        public static string _modelPath = Path.Combine(Directory.GetCurrentDirectory(), "DataFiles", "model.zip");

        public static void Train(IEnumerable<ProductData> data)
        {
            MLContext context = new MLContext();

            var pipeline = context.Transforms.Text.FeaturizeText("Specifications", "Specifications")
                .Append(context.Transforms.Conversion.ConvertType("Warranty", "Warranty", DataKind.Single))
                .Append(context.Transforms.Categorical.OneHotEncoding("CategoryName", "CategoryName"))
                .Append(context.Transforms.Categorical.OneHotEncoding("BrandName", "BrandName"))
                .Append(context.Transforms.Concatenate("Features", "Price", "Warranty", "CategoryName", "BrandName"))
                .Append(context.Clustering.Trainers.KMeans("Features", numberOfClusters: 20));

            var trainingData = context.Data.LoadFromEnumerable(data);
            ITransformer model = pipeline.Fit(trainingData);

            context.Model.Save(model, trainingData.Schema, _modelPath);
        }

        public static ProductClusterPrediction? Predict(ProductData query)
        {
            MLContext context = new MLContext();

            var model = context.Model.Load(_modelPath, out var schema);

            var data = new List<ProductData> { query };
            var dataView = context.Data.LoadFromEnumerable(data);

            var predictions = model.Transform(dataView);

            return context.Data.CreateEnumerable<ProductClusterPrediction>(predictions, reuseRowObject: false).FirstOrDefault();
        }

        public static IEnumerable<ProductClusterPrediction> Predict(List<ProductData> data)
        {
            MLContext context = new MLContext();

            var model = context.Model.Load(_modelPath, out var schema);

            var dataView = context.Data.LoadFromEnumerable(data);

            var predictions = model.Transform(dataView);

            return context.Data.CreateEnumerable<ProductClusterPrediction>(predictions, reuseRowObject: false);
        }
    }
}
