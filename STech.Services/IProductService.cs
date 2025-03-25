﻿using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface IProductService
    {
        Task<int> GetTotalProducts();
        Task<int> GetMonthAdded(int month);
        Task<IEnumerable<Product>> GetProducts();
        Task<PagedList<Product>> GetProducts(string? brands, string? categories, string? status, string? priceRange, 
            string? warehouseId, string? sort, int page, int itemsPerPage);
        Task<PagedList<Product>> SearchByName(string q, int page, int itemsPerPage, string? sort);
        Task<PagedList<Product>> SearchProducts(string q, int page, int itemsPerPage, string? sort, string? warehouseId);
        Task<PagedList<Product>> SearchProductsByIdOrName(string q, int page, int itemsPerPage);
        Task<IEnumerable<Product>> SearchProductsByChatBotData(List<ProductSpecification>? specs, string? productId, string? productName, string? priceStr);
        Task<IEnumerable<Product>> SearchProductsByBrandName(string brandName);
        Task<IEnumerable<Product>> SearchProductsByCategoryName(string categoryName);
        Task<PagedList<Product>> GetByCategory(string categoryId, string? brands, string? priceRange,
            int page, int itemsPerPage, string? sort);
        Task<IEnumerable<Product>> GetSimilarProducts(string categoryId, int numToTake);
        Task<PagedList<Product>> GetBestSellingProducts(int page, int itemsPerPage);
        Task<IEnumerable<Product>> GetBestSellingProducts(int numToTake);
        Task<PagedList<Product>> GetNewestProducts(int page, int itemsPerPage);
        Task<Product?> GetProduct(string id);
        Task<Product?> GetProductWithBasicInfo(string id);
        Task<Product?> GetProductWithBasicInfo(string id, string warehouseId);
        Task<bool> CheckOutOfStock(string id);
        Task<int> GetTotalQty(string id);

        Task<bool> DeleteProduct(string id);
        Task<bool> DeleteProducts(string[] ids);
        Task<bool> PermanentlyDeleteProduct(string id);
        Task<bool> PermanentlyDeleteProducts(string[] ids);

        Task<bool> RestoreProduct(string id);
        Task<bool> RestoreProducts(string[] ids);

        Task<bool> ActivateProduct(string id);
        Task<bool> ActivateProducts(string[] ids);

        Task<bool> DeActivateProduct(string id);
        Task<bool> DeActivateProducts(string[] ids);

        Task<bool> UpdateProduct(ProductVM product);
        Task<bool> CreateProduct(ProductVM product);
    }
}
