using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vega.Controllers.Resources;
using Vega.Core;
using Vega.Core.Domain;
using Vega.Persistence.Repositories;
using Vega.Utilities;

namespace Vega.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class VehiclesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string[] _permittedExtensions = {".jpg"};
        private readonly string _targetFilePath;
        private readonly long _fileSizeLimit;
        private readonly string _requestPath;

        public VehiclesController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _targetFilePath = config.GetValue<string>("StoredFilePath");
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            _requestPath = config.GetValue<string>("RequestFilePath");
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicles([FromQuery] VehicleParameters vehicleParameters)
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllVehiclesWithInfoAsync(vehicleParameters);
            var result = vehicles.Select(_mapper.Map<Vehicle, VehicleResource>);

            var metaData = new
            {
                vehicles.TotalCount,
                vehicles.PageSize,
                vehicles.CurrentPage,
                vehicles.TotalPage,
                vehicles.HasNext,
                vehicles.HasPrevious
            };
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(metaData, Formatting.None, serializerSettings));

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var vehicle = await _unitOfWork.Vehicles.GetVehicleWithInfoAsync(id);
            if (vehicle == null)
                return NotFound();

            var result = _mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] SaveVehicleResource vehicleResource)
        {
            var vehicle = _mapper.Map<SaveVehicleResource, Vehicle>(vehicleResource);
            vehicle.LastUpdate = DateTime.Now;
            await _unitOfWork.Vehicles.AddAsync(vehicle);
            await _unitOfWork.CompleteAsync();

            var addedVehicle = await _unitOfWork.Vehicles.GetVehicleWithInfoAsync(vehicle.Id);
            var result = _mapper.Map<Vehicle, VehicleResource>(addedVehicle);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _unitOfWork.Vehicles.GetAsync(id);
            if (vehicle == null)
                return NotFound();

            _unitOfWork.Vehicles.Remove(vehicle);
            await _unitOfWork.CompleteAsync();

            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] SaveVehicleResource vehicleResource)
        {
            var vehicle = await _unitOfWork.Vehicles.GetVehicleWithInfoAsync(id);
            if (vehicle == null)
                return NotFound();

            _mapper.Map(vehicleResource, vehicle);
            await _unitOfWork.CompleteAsync();

            vehicle = await _unitOfWork.Vehicles.GetVehicleWithInfoAsync(id);
            var result = _mapper.Map<Vehicle, VehicleResource>(vehicle);
            return Ok(result);
        }

        [HttpPost("photos/{id}")]
        public async Task<IActionResult> UploadPhotos(int id, List<IFormFile> files)
        {
            var vehicle = await _unitOfWork.Vehicles.GetAsync(id);
            if (vehicle == null)
                return NotFound();

            foreach (var formFile in files)
            {
                var formFileContent = await FileHelpers.ProcessFormFile<VehicleResource>(
                    formFile, ModelState, _permittedExtensions, _fileSizeLimit);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var trustedFileNameForFileStorage =
                    $"{Path.GetRandomFileName()}{Guid.NewGuid()}".GetHashCode().ToString();
                var completeTrustedFileName = $"{trustedFileNameForFileStorage}{Path.GetExtension(formFile.FileName)}";
                var photoDirectory = Path.Combine("VehiclePhotos", id.ToString());
                
                var physicalStorageDirectory = Path.Combine(_targetFilePath, photoDirectory);
                // Physical path to store the file.
                var filePath = Path.Combine(physicalStorageDirectory, completeTrustedFileName);
                // The path to request the file.
                var requestFilePath = Path.Combine(_requestPath, photoDirectory, completeTrustedFileName).Replace('\\', '/');

                if (!Directory.Exists(physicalStorageDirectory))
                    Directory.CreateDirectory(physicalStorageDirectory);

                await using var fileStream = System.IO.File.Create(filePath);
                await fileStream.WriteAsync(formFileContent);

                // Write into database after the file is uploaded successfully.
                var photo = new Photo
                {
                    FilePath = filePath,
                    RequestPath = requestFilePath
                };
                vehicle.Photos.Add(photo);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        [HttpGet("photos/{id}")]
        public IActionResult GetPhotos(int id)
        {
            var photos = _unitOfWork.Photos.Find(p => p.VehicleId == id);

            return Ok(_mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoResource>>(photos));
        }
    }
}