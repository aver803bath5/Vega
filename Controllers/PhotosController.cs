using System.Collections.Generic;
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
        private readonly IPhotoService _photoService;
        private readonly string[] _permittedExtensions = {".jpg", ".jpeg", ".png", ".gif"};
        private readonly string _targetFilePath;
        private readonly long _fileSizeLimit;

        public PhotosController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config,
            IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
            _targetFilePath = config.GetValue<string>("StoredFilePath");
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        }


        [HttpPost]
        [Authorize]
        // Use a list collection to get the uploading files so that I don't need to change the code if client-side
        // decide to upload multiple photos at once.
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

                var photo = await _photoService.UploadPhoto(vehicle, formFile, formFileContent, _targetFilePath);

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

        [HttpDelete("{photoId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int photoId)
        {
            var photo = await _unitOfWork.Photos.GetAsync(photoId);
            if (photo == null)
                return NotFound();

            await _photoService.DeletePhoto(photo, _targetFilePath);

            return Ok(_mapper.Map<Photo, PhotoResource>(photo));
        }
    }
}