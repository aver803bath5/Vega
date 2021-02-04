using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vega.Models;
using Vega.Persistence;

namespace Vega.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        private readonly VegaDbContext _context;

        public FeaturesController(VegaDbContext context)
        {
            _context = context;
        }

        [HttpGet("/api/features")]
        public async Task<IEnumerable<Feature>> GetFeatures()
        {
            var features = await _context.Features.ToListAsync();
            return features;
        }
    }
}
