using System.Collections.Generic;
using System.Threading.Tasks;
using Vega.Controllers.Resources;
using Vega.Core.Domain;

namespace Vega.Core.Repositories
{
    public interface IMakeRepository : IRepository<Make>
    {
        Task<IEnumerable<Make>> GetMakesWithModelAsync();
    }
}