using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Vega.Core
{
    public interface IPhotoStorage
    {
        // Store vehicle photo file into server machine and return the photo file name in the machine.
        Task<string> StorePhoto(string uploadsFolderPath, IFormFile file, byte[] fileContent);

        // Create and store vehicle photos' thumbnail files.
        // Return the filepath of the thumbnail.
        string StoreThumbnail(string uploadFolderPath);

        void DeletePhoto(string filePath);
        
        // Delete the directory storing vehicle photos and the photo files inside it.
        void DeleteDirectory(string directory);
    }
}