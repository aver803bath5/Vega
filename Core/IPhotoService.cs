using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vega.Core.Domain;

namespace Vega.Core
{
    public interface IPhotoService
    {
        // This method is used to store vehicle photos to the server machine
        // and insert the data into Photos table.
        Task<Photo> UploadPhoto(Vehicle vehicle, IFormFile file, byte[] formFileContent, string targetFilePath);
    }
}