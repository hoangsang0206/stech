﻿using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services
{
    public interface IProductService
    {
        public Task<IEnumerable<Product>> GetAll();
        public Task<IEnumerable<Product>> SearchByName(string q);
        public Task<IEnumerable<Product>> GetByCategory(string categoryId);
    }
}
