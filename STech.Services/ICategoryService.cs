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
        public Task<IEnumerable<Category>> GetAll(bool isExcept);
        public Task<IEnumerable<Category>> GetRandomWithProducts(int numCategories, int numProducts);
        public Task<Category> GetOne(string id);
    }
}
