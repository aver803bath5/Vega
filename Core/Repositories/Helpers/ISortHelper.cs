using System.Collections.Generic;
using System.Linq;

namespace Vega.Core.Repositories.Helpers
{
    public interface ISortHelper<T>
    {
        IQueryable<T> ApplySort(IQueryable<T> entities, Dictionary<string, string> columnsMapping, string orderByQueryString);
    }
}