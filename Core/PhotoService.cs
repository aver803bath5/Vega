using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vega.Core.Domain;

namespace Vega.Core
{
    public class PhotoService : IPhotoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PhotoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // This method is used to store vehicle photos to the server machine
        // and insert the data into Photos table.
        public async Task<Photo> UploadPhoto(Vehicle vehicle, IFormFile formFile, byte[] formFileContent,
            string targetFilePath)
        {
            // Generate a trusted file name for the file being stored in the machine.
            // It is a overkill. It's OK to only use GUID to generate the file name to avoid the chance to
            // create multiple files with same name. I use `GetHashCode` just to make the file name shorter.
            var trustedFileNameForFileStorage =
                $"{Path.GetRandomFileName()}{Guid.NewGuid()}".GetHashCode().ToString();
            var completeTrustedFileName = $"{trustedFileNameForFileStorage}{Path.GetExtension(formFile.FileName)}";

            // Create a folder to store vehicle folders individually. Every vehicle photos will be stored in the folder
            // named by the vehicle id to reduce the chance to be in the same folder with the file with same name.
            var photoDirectory = Path.Combine("VehiclePhotos", vehicle.Id.ToString());
            var physicalStoragePath = Path.Combine(targetFilePath, photoDirectory);
            if (!Directory.Exists(physicalStoragePath))
                Directory.CreateDirectory(physicalStoragePath);

            // Physical path to store the file.
            var filePath = Path.Combine(physicalStoragePath, completeTrustedFileName);
            // Write file to physical storage.
            await using var fileStream = File.Create(filePath);
            await fileStream.WriteAsync(formFileContent);

            // Write into database after the file is uploaded successfully.
            var photo = new Photo
            {
                FileName = completeTrustedFileName
            };
            vehicle.Photos.Add(photo);
            await _unitOfWork.CompleteAsync();

            return photo;
        }
    }
}