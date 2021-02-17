using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vega.Core.Domain;
using Vega.Core.Repositories;

namespace Vega.Persistence.Repositories
{
    public class MakeRepository : Repository<Make>, IMakeRepository
    {
        private readonly VegaDbContext _context;

        public MakeRepository(VegaDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Make>> GetMakesWithModelAsync()
        {
            return await _context.Makes.Include(m => m.Models).ToListAsync();
        }
    }
}