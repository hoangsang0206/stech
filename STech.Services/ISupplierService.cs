using STech.Data.Models;

namespace STech.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetSuppliers();
        Task<Supplier?> GetSupplierById(string id);
        Task<Supplier?> GetSupplierByIdWithImports(string id);
        
        Task<bool> CreateSupplier(Supplier supplier);
        Task<bool> UpdateSupplier(Supplier supplier);
        Task<bool> DeleteSupplier(string id);
    }
}
