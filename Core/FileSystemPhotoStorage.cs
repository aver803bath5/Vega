using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Vega.Core
{
    public class FileSystemPhotoStorage : IPhotoStorage
    {
        public async Task<string> StorePhoto(string uploadsFolderPath, IFormFile file, byte[] fileContent)
        {
            // Generate a trusted file name for the file being stored in the machine.
            // It is a overkill. It's OK to only use GUID to generate the file name to avoid the chance to
            // create multiple files with same name. I use `GetHashCode` just to make the file name shorter.
            var trustedFileNameForFileStorage =
                $"{Path.GetRandomFileName()}{Guid.NewGuid()}".GetHashCode().ToString();
            var completeTrustedFileName = $"{trustedFileNameForFileStorage}{Path.GetExtension(file.FileName)}";

            // Physical path to store the file.
            var filePath = Path.Combine(uploadsFolderPath, completeTrustedFileName);
            // Write file to physical storage.
            await using var fileStream = File.Create(filePath);
            await fileStream.WriteAsync(fileContent);

            return completeTrustedFileName;
        }

        public string StoreThumbnail(string originImageFilePath)
        {
            // Create vehicle photo image's thumbnail.
            var image = Image.FromFile(originImageFilePath);
            var thumb = image.GetThumbnailImage(200, 200, () => false, IntPtr.Zero);

            // Add 'thumbnail' string at the end of the original image file name to differ the thumbnail filename from 
            // original image filename.
            var thumbnailFileName = Path.GetFileNameWithoutExtension(originImageFilePath) + "thumbnail" + ".jpeg";
            var physicalStorageDirectory = Path.GetDirectoryName(originImageFilePath);
            var thumbNailFilePath = Path.Combine(physicalStorageDirectory ?? string.Empty, $"{thumbnailFileName}");
            thumb.Save(thumbNailFilePath, ImageFormat.Jpeg); // Save thumbnail image file as jpeg format.

            return thumbNailFilePath;
        }

        public void DeletePhoto(string filePath)
        {
            File.Delete(filePath);
        }

        public void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }
    }
}