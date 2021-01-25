using Microsoft.AspNetCore.Mvc;

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
    }
}
