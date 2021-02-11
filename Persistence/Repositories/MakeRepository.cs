using Vega.Core.Domain;
using Vega.Core.Repositories;

namespace Vega.Persistence.Repositories
{
    public class MakeRepository : Repository<Make>, IMakeRepository
    {
        public MakeRepository(VegaDbContext context)
            : base(context)
        {
        }
    }
}