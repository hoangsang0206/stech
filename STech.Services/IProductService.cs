using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> SearchByName(string q);
        Task<IEnumerable<Product>> GetByCategory(string categoryId);
    }
}
