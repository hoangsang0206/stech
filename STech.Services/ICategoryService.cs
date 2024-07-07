using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll(bool isExcept);
        Task<IEnumerable<Category>> GetRandomWithProducts(int numCategories, int numProducts);
        Task<Category> GetOne(string id);
    }
}
