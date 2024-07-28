using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services
{
    public interface IWarehouseService
    {
        Task<Warehouse?> GetOnlineWarehouse();

        Task<bool> SubtractProductQuantity(string warehouseId, string productId, int quantity);
    }
}
