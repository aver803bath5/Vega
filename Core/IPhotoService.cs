using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vega.Core.Domain;

namespace Vega.Core
{
    public interface IPhotoService
    {
        // This method is used to store vehicle photos on the server machine
        // and insert the data into Photos table.
        Task<Photo> UploadPhoto(Vehicle vehicle, IFormFile file, byte[] formFileContent, string targetFilePath);

        // This method is used to delete vehicle photos on the server machine.
        // And remove the vehicle photo data from the database table.
        Task<Photo> DeletePhoto(Photo photo, string targetFilePath);

        void DeletePhotosDirectory(int vehicleId, string targetFilePath);
    }
}