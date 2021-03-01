using System.Threading.Tasks;
using Vega.Core;
using Vega.Core.Domain;
using Vega.Core.Repositories;
using Vega.Core.Repositories.Helpers;
using Vega.Persistence.Repositories;

namespace Vega.Persistence
{
    public class UnitOfWork : IUnitOfWork

    {
        private readonly VegaDbContext _context;
        private readonly ISortHelper<Vehicle> _vehicleSortHelper;

        public UnitOfWork(VegaDbContext context, ISortHelper<Vehicle> vehicleSortHelper)
        {
            _context = context;
            _vehicleSortHelper = vehicleSortHelper;
            Features = new FeatureRepository(_context);
            Makes = new MakeRepository(_context);
            Vehicles = new VehicleRepository(_context, vehicleSortHelper);
        }

        public IFeatureRepository Features { get; }
        public IMakeRepository Makes { get; }
        public IVehicleRepository Vehicles { get; }

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