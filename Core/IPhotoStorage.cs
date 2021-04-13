using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Vega.Core
{
    public interface IPhotoStorage
    {
        // Store vehicle photo file into server machine and return the photo file name in the machine.
        Task<string> StorePhoto(string uploadsFolderPath, IFormFile file, byte[] fileContent);

        void DeletePhoto(string filePath);
    }
}