using Vega.Core.Domain;
using Vega.Core.Repositories;

namespace Vega.Persistence.Repositories
{
    public class FeatureRepository : Repository<Feature>, IFeatureRepository

    {
        public FeatureRepository(VegaDbContext context)
        :base(context)
        {
        }
    }
}