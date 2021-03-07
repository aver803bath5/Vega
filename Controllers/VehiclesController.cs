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

        public VehiclesController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _targetFilePath = config.GetValue<string>("StoredFilePath");
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
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
                var directory = Path.Combine(_targetFilePath, "VehiclePhotos", id.ToString());
                var filePath = Path.Combine(directory, trustedFileNameForFileStorage);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                await using var fileStream = System.IO.File.Create(filePath + Path.GetExtension(formFile.FileName));
                await fileStream.WriteAsync(formFileContent);
            }

            return Ok(new {id, files});
        }
    }
}