using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vega.Dtos;
using Vega.Models;

namespace Vega.Controllers
{
    [Route("api/makes")]
    [ApiController]
    public class MakesController
    {
        private readonly VegaDbContext _context;
        private readonly IMapper _mapper;

        public MakesController(VegaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("/api/makes")]
        public async Task<IEnumerable<MakeDto>> GetMakes()
        {
            var makes = await _context.Makes
                .Include(m => m.Models)
                .ToListAsync();
            var makesDto = makes.Select(_mapper.Map<Make, MakeDto>);
            return makesDto;
        }
    }
}