using System.Collections.Generic;
using System.Threading.Tasks;
using Vega.Core.Domain;

namespace Vega.Core.Repositories
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesWithInfoAsync();
        Task<Vehicle> GetVehicleWithInfoAsync(int id);
    }
}