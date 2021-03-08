using Vega.Core.Domain;
using Vega.Core.Repositories;

namespace Vega.Persistence.Repositories
{
    public class PhotoRepository : Repository<Photo>, IPhotoRepository
    {
        public PhotoRepository(VegaDbContext context)
            : base(context)
        {
        }
    }
}