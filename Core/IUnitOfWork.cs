using System;
using System.Threading.Tasks;
using Vega.Core.Repositories;
using Vega.Persistence;

namespace Vega.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IFeatureRepository Features { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}