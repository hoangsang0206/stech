using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Services
{
    public class SliderService : ISliderService
    {
        private readonly StechDbContext _context;
        public SliderService(StechDbContext context) => _context = context;

        public async Task<IEnumerable<Slider>> GetAll()
        {
            return await _context.Sliders.ToListAsync();
        } 
    }
}
