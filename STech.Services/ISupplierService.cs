using STech.Data.Models;

namespace STech.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetSuppliers();
    }
}
