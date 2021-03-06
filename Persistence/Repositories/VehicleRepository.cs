using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vega.Core.Domain;
using Vega.Core.Repositories;
using Vega.Core.Repositories.Helpers;

namespace Vega.Persistence.Repositories
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        private readonly VegaDbContext _context;
        private readonly ISortHelper<Vehicle> _sortHelper;

        public VehicleRepository(VegaDbContext context, ISortHelper<Vehicle> sortHelper)
            : base(context)
        {
            _context = context;
            _sortHelper = sortHelper;
        }

        public async Task<PagedList<Vehicle>> GetAllVehiclesWithInfoAsync(VehicleParameters vehicleParameters)
        {
            var vehicles = _context.Vehicles.AsQueryable();

            // If request URI has makeId query parameter which value is greater than 1 which mean it is valid MakeId,
            // filter the vehicles with MakeId.
            if (vehicleParameters.MakeId > 0)
                vehicles = Find(v => v.Model.MakeId == vehicleParameters.MakeId).AsQueryable();

            var columnsMap = new Dictionary<string, string>()
            {
                ["make"] = "model.make.name",
                ["model"] = "model.name",
                ["contactName"] = "contact.name",
                ["id"] = "id"
            };
            var sortedVehicles = _sortHelper.ApplySort(vehicles, columnsMap, vehicleParameters.OrderBy);

            return await PagedList<Vehicle>.ToPagedListAsync(sortedVehicles
                    .Include(v => v.Features)
                    .ThenInclude(vf => vf.Feature)
                    .Include(v => v.Model)
                    .ThenInclude(m => m.Make).AsQueryable()
                , vehicleParameters.PageNumber, vehicleParameters.PageSize);
        }

        public async Task<Vehicle> GetVehicleWithInfoAsync(int id)
        {
            return await _context.Vehicles
                .Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .SingleOrDefaultAsync(v => v.Id == id);
        }
    }
}