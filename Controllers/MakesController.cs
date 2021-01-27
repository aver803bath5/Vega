using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vega.Models;

namespace Vega.Controllers
{
    [Route("api/makes")]
    [ApiController]
    public class MakesController
    {
        private readonly VegaDbContext _context;

        public MakesController(VegaDbContext context)
        {
            _context = context;
        }

        [HttpGet("/api/makes")]
        public async Task<IEnumerable<Make>> GetMakes()
        {
            var makes = await _context.Makes
                .Include(m => m.Models)
                .ToListAsync();
            return makes;
        }
    }
}
