using System.Threading.Tasks;
using Vega.Core;
using Vega.Core.Repositories;
using Vega.Persistence.Repositories;

namespace Vega.Persistence
{
    public class UnitOfWork : IUnitOfWork

    {
        private readonly VegaDbContext _context;

        public UnitOfWork(VegaDbContext context)
        {
            _context = context;
            Features = new FeatureRepository(_context);
            Makes = new MakeRepository(_context);
        }

        public IFeatureRepository Features { get; private set; }
        public IMakeRepository Makes { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}