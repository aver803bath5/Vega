using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Vega.Models;

namespace Vega.Controllers
{
    [Route("api/makes")]
    [ApiController]
    public class MakesControllers
    {
        private readonly VegaDbContext _context;

        public MakesControllers(VegaDbContext context)
        {
            _context = context;
        }

        [HttpGet("/api/makes")]
        public IEnumerable<Make> GetMakes()
        {
            var makes = _context.Makes.ToList();
            return makes;
        }
    }
}
