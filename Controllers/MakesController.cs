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
    [ApiController]
    [Route("/api/[controller]")]
    public class MakesController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MakesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<MakeResource>> GetMakes()
        {
            var makes = await _unitOfWork.Makes.GetMakesWithModelAsync();
            return _mapper.Map<List<Make>, List<MakeResource>>(makes.ToList());
        }
    }
}