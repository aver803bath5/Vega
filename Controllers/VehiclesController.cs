using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vega.Controllers.Resources;
using Vega.Core;
using Vega.Core.Domain;
using Vega.Persistence.Repositories;

namespace Vega.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class VehiclesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _targetFilePath;

        public VehiclesController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _targetFilePath = config.GetValue<string>("StoredFilePath");
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
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _unitOfWork.Vehicles.GetAsync(id);
            if (vehicle == null)
                return NotFound();
            
            // Remove all photos data of the vehicle. Delete from database.
            var photos = _unitOfWork.Photos.Find(p => p.VehicleId == id);
            _unitOfWork.Photos.RemoveRange(photos);
            
            // Remove the directory and the photo files of the vehicles.
            var photosDirectory = Path.Combine(_targetFilePath, "VehiclePhotos", id.ToString());
            if (Directory.Exists(photosDirectory))
                Directory.Delete(photosDirectory, true);

            _unitOfWork.Vehicles.Remove(vehicle);
            await _unitOfWork.CompleteAsync();

            return Ok(id);
        }

        [HttpPut("{id}")]
        [Authorize]
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
    }
}