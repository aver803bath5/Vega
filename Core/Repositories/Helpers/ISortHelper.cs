using System.Linq;

namespace Vega.Core.Repositories.Helpers
{
    public interface ISortHelper<T>
    {
        IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString);
    }
}