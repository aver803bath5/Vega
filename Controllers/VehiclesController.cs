using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vega.Controllers.Resources;
using Vega.Core;
using Vega.Core.Domain;

namespace Vega.Controllers
{
    public class VehiclesParameters : QueryStringParameters
    {
        public int MakeId { get; set; }
    }

    [ApiController]
    [Route("/api/[controller]")]
    public class VehiclesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VehiclesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicles([FromQuery] VehiclesParameters vehiclesParameters)
        {
            if (vehiclesParameters.MakeId > 0)
            {
                var filteredVehicles = await _unitOfWork.Vehicles.FilterWithMakeAsync(vehiclesParameters.MakeId);
                return Ok(_mapper.Map<IEnumerable<Vehicle>, IEnumerable<VehicleResource>>(filteredVehicles));
            }

            if (!string.IsNullOrWhiteSpace(vehiclesParameters.OrderBy))
            {
                var orderedVehicles = await _unitOfWork.Vehicles.OrderByParameter(vehiclesParameters.OrderBy);
                return Ok(_mapper.Map<IEnumerable<Vehicle>, IEnumerable<VehicleResource>>(orderedVehicles));
            }

            var vehicles = await _unitOfWork.Vehicles.GetAllVehiclesWithInfoAsync();
            var result = vehicles.Select(_mapper.Map<Vehicle, VehicleResource>);

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
    }
}