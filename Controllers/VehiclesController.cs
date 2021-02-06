using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vega.Controllers.Resources;
using Vega.Models;
using Vega.Persistence;

namespace Vega.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly VegaDbContext _context;
        private readonly IMapper _mapper;

        public VehiclesController(VegaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("/api/vehicles")]
        public async Task<IActionResult> GetVehicles()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Features)
                .ThenInclude(f => f.Feature)
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .ToListAsync();

            var result = vehicles.Select(_mapper.Map<Vehicle, VehicleResource>);
            
            return Ok(result);
        }
        
        [HttpGet("/api/vehicles/{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id);
            if (vehicle == null)
                return NotFound();
            
            await _context.Vehicles
                .Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .LoadAsync();
            var result = _mapper.Map<Vehicle, VehicleResource>(vehicle);
                
            return Ok(result);
        }

        [HttpPost("/api/vehicles")]
        public async Task<IActionResult> CreateVehicle([FromBody] SaveVehicleResource vehicleResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vehicle = _mapper.Map<SaveVehicleResource, Vehicle>(vehicleResource);
            vehicle.LastUpdate = DateTime.Now;
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();

            var addedVehicle = await _context.Vehicles
                .Include(v => v.Features)
                .ThenInclude(f => f.Feature)
                .Include(v => v.Model)
                .ThenInclude(m => m.Make)
                .FirstOrDefaultAsync(v => v.Id == vehicle.Id);
            
            var result = _mapper.Map<Vehicle, VehicleResource>(addedVehicle);
            return Ok(result);
        }

        [HttpDelete("/api/vehicles/{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return NotFound();
            
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        [HttpPut("/api/vehicles")]
        public async Task<IActionResult> UpdateVehicle([FromBody] SaveVehicleResource vehicleResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var vehicle = await _context.Vehicles
                .Include(v => v.Features)
                .FirstOrDefaultAsync(v => v.Id == vehicleResource.Id);
            if (vehicle == null)
                return NotFound();
            
            _mapper.Map(vehicleResource, vehicle);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
    }
}