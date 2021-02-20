using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Vega.Core.Domain;
using Vega.Core.Repositories;

namespace Vega.Persistence.Repositories
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        private readonly VegaDbContext _context;

        public VehicleRepository(VegaDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesWithInfoAsync()
        {
            return await _context.Vehicles
                .Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .ToListAsync();
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

        public async Task<IEnumerable<Vehicle>> FilterWithMakeAsync(int makeId)
        {
            return await _context.Vehicles.Where(v => v.Model.MakeId == makeId)
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> OrderByParameter(string orderByQueryString)
        {
            var vehicles = _context.Vehicles
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
                .AsQueryable();
            
            ApplySort(ref vehicles, orderByQueryString);
            return await vehicles.ToListAsync();
        }

        // orderByQueryString: 'contactName,make desc'
        private void ApplySort(ref IQueryable<Vehicle> vehicles, string orderByQueryString)
        {
            if (!vehicles.Any())
                return;

            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                vehicles = vehicles.OrderBy(v => v.Id);
                return;
            }

            var orderParams = orderByQueryString.Trim().Split(',');
            // TODO: 是否需要檢查要 sort 的 class 的 property 有沒有包含在 orderByQueryString 中
            // 因為 Vehicle 存在一個 owned entity，代表需要遞迴去檢查，而且取出 owned entity 的 properties 之後，
            // 較需要遞迴檢查 queryString 是否有包含在 owned entity 的 property 內，eg queryString: contact.name
            // owned entity 為 Contact { Name; Phone; }
            // 目前先不做檢查，直接扔出 exception，因為檢查會讓程式更複雜，而且對目前的開發來說，價值不大，因為目前 request
            // 是從 client 端的操作傳過來的，如果出現意料之外的情況就是不當操作。
            // var propertyInfos = typeof(Vehicle).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(" ")[0];
                // var objectProperty = propertyInfos.FirstOrDefault(pi =>
                //     pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                //
                // if (objectProperty == null)
                //     continue;

                var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";

                orderQueryBuilder.Append($"{propertyFromQueryName} {sortingOrder}");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

            // if (string.IsNullOrWhiteSpace(orderQuery))
            // {
            //     vehicles = vehicles.OrderBy(x => x.Id);
            //     return;
            // }

            vehicles = vehicles.OrderBy(orderQuery);
        }
    }
}