using System.Collections.Generic;
using System.Threading.Tasks;
using Vega.Controllers;
using Vega.Core.Domain;
using Vega.Persistence.Repositories;

namespace Vega.Core.Repositories
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<PagedList<Vehicle>> GetAllVehiclesWithInfoAsync(VehiclesParameters vehiclesParameters);
        Task<Vehicle> GetVehicleWithInfoAsync(int id);
    }
}