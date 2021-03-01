using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Vega.Core.Repositories.Helpers
{
    public class SortHelper<T> : ISortHelper<T>
    {
        public IQueryable<T> ApplySort(IQueryable<T> entities, Dictionary<string, string> columnsMapping, string orderByQueryString)
        {
            if (!entities.Any())
                return entities;

            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return entities;

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

                var propertyFromQueryName = columnsMapping[param.Split(' ')[0]];
                var sortOrder = param.EndsWith("desc") ? "descending" : "ascending";
                // var objectProperty = propertyInfos.FirstOrDefault(pi =>
                //     pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                //
                // if (objectProperty == null)
                //     continue;

                orderQueryBuilder.Append($"{propertyFromQueryName} {sortOrder}");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

            // if (string.IsNullOrWhiteSpace(orderQuery))
            // {
            //     vehicles = vehicles.OrderBy(x => x.Id);
            //     return;
            // }

            return entities.OrderBy(orderQuery);
        }
    }
}