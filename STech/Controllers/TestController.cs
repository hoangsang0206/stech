using STech.TrainModel;
using Microsoft.AspNetCore.Mvc;
using STech.Data.TrainingDataModels;
using STech.Services;

namespace STech.Controllers
{
    public class TestController : Controller
    {
        private readonly IProductService _productService;

        public TestController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {

            IEnumerable<ProductData> data = await _productService.GetTrainingData();
            KMeans.Train(data);
            var predict = KMeans.Predict(new ProductData
            {
                //CategoryName = "Laptop",
                //BrandName = "ACER",
                //Price = 500000,
                //Warranty = 12,
                Specifications = "[RAM: 32GB]"
            });

            var productClusters = KMeans.Predict(data.ToList());

            var result = productClusters.Where(x => x.PredictedLabel == predict?.PredictedLabel).Select(x => new
            {
                x.ProductId,
                x.Price,
                x.PredictedLabel
            }).ToList();

            return View();
        }
    }
}
