using System.Collections.Generic;
using System.Threading.Tasks;
using Vega.Core.Domain;

namespace Vega.Core.Repositories
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesWithInfoAsync();
        Task<Vehicle> GetVehicleWithInfoAsync(int id);
        Task<IEnumerable<Vehicle>> FilterWithMakeAsync(int makeId);
        Task<IEnumerable<Vehicle>> OrderByParameter(string orderByQueryString);
    }
}