using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vega.Controllers;
using Vega.Core.Domain;

namespace Vega.Core
{
    public class PhotoService : IPhotoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoStorage _photoStorage;

        public PhotoService(IUnitOfWork unitOfWork, IPhotoStorage photoStorage)
        {
            _unitOfWork = unitOfWork;
            _photoStorage = photoStorage;
        }

        // This method is used to store vehicle photos to the server machine
        // and insert the data into Photos table.
        public async Task<Photo> UploadPhoto(Vehicle vehicle, IFormFile formFile, byte[] formFileContent,
            string targetFilePath)
        {
            // Create a folder to store vehicle folders individually. Every vehicle photos will be stored in the folder
            // named by the vehicle id to reduce the chance to be in the same folder with the file with same name.
            var photoDirectory = Path.Combine(FilePaths.VehiclePhotosDirectory, vehicle.Id.ToString());
            var physicalStoragePath = Path.Combine(targetFilePath, photoDirectory);
            if (!Directory.Exists(physicalStoragePath))
                Directory.CreateDirectory(physicalStoragePath);

            // Store the file into the server machine and return the file name in the server machine.
            var fileName = await _photoStorage.StorePhoto(physicalStoragePath, formFile, formFileContent);

            // Create thumbnail for vehicle image and save it to the server.
            var originImageFilePath = Path.Combine(physicalStoragePath, fileName);
            var thumbNailFilePath = _photoStorage.StoreThumbnail(originImageFilePath);

            // Write into database after the file is uploaded successfully.
            var photo = new Photo
            {
                FileName = fileName,
                Thumbnail = Path.GetFileName(thumbNailFilePath)
            };
            vehicle.Photos.Add(photo);
            await _unitOfWork.CompleteAsync();

            return photo;
        }

        public async Task<Photo> DeletePhoto(Photo photo, string targetFilePath)
        {
            // Remove photo file.
            var vehicleId = photo.VehicleId;
            var photoFilePath = Path.Combine(targetFilePath, FilePaths.VehiclePhotosDirectory, vehicleId.ToString(),
                photo.FileName);
            _photoStorage.DeletePhoto(photoFilePath);

            // Remove photo data from database.
            _unitOfWork.Photos.Remove(photo);
            await _unitOfWork.CompleteAsync();

            return photo;
        }

        public void DeletePhotosDirectory(int vehicleId, string targetFilePath)
        {
            // Remove all photos data of the vehicle. Delete from database.
            var photos = _unitOfWork.Photos.Find(p => p.VehicleId == vehicleId);
            _unitOfWork.Photos.RemoveRange(photos);

            // Remove the directory and the photo files of the vehicles.
            var photosDirectory = Path.Combine(targetFilePath, FilePaths.VehiclePhotosDirectory, vehicleId.ToString());
            _photoStorage.DeleteDirectory(photosDirectory);
        }
    }
}