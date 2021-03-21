using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Vega.Controllers.Resources;
using Vega.Core;
using Vega.Core.Domain;
using Vega.Utilities;

namespace Vega.Controllers
{
    [ApiController]
    [Route("/api/vehicles/{vehicleId}/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string[] _permittedExtensions = {".jpg", ".jpeg", ".png", ".gif"};
        private readonly string _targetFilePath;
        private readonly long _fileSizeLimit;

        public PhotosController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _targetFilePath = config.GetValue<string>("StoredFilePath");
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload(int vehicleId, List<IFormFile> files)
        {
            var vehicle = await _unitOfWork.Vehicles.GetAsync(vehicleId);
            var result = new List<Photo>();
            if (vehicle == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            foreach (var formFile in files)
            {
                var formFileContent = await FileHelpers.ProcessFormFile<VehicleResource>(
                    formFile, ModelState, _permittedExtensions, _fileSizeLimit);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var trustedFileNameForFileStorage =
                    $"{Path.GetRandomFileName()}{Guid.NewGuid()}".GetHashCode().ToString();
                var completeTrustedFileName = $"{trustedFileNameForFileStorage}{Path.GetExtension(formFile.FileName)}";
                var photoDirectory = Path.Combine("VehiclePhotos", vehicleId.ToString());

                var physicalStoragePath = Path.Combine(_targetFilePath, photoDirectory);
                if (!Directory.Exists(physicalStoragePath))
                    Directory.CreateDirectory(physicalStoragePath);

                // Physical path to store the file.
                var filePath = Path.Combine(physicalStoragePath, completeTrustedFileName);
                // Write file to physical storage.
                await using var fileStream = System.IO.File.Create(filePath);
                await fileStream.WriteAsync(formFileContent);

                // Write into database after the file is uploaded successfully.
                var photo = new Photo
                {
                    FileName = completeTrustedFileName
                };
                vehicle.Photos.Add(photo);
                await _unitOfWork.CompleteAsync();
                result.Add(photo);
            }

            return Ok(_mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoResource>>(result));
        }

        [HttpGet]
        public IActionResult GetPhotos(int vehicleId)
        {
            var photos = _unitOfWork.Photos.Find(p => p.VehicleId == vehicleId);

            return Ok(_mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoResource>>(photos));
        }
    }
}