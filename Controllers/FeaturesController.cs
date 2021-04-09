using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Vega.Controllers.Resources;
using Vega.Core;
using Vega.Core.Domain;

namespace Vega.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class FeaturesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeaturesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeatures()
        {
            // 123
            var features = await _unitOfWork.Features.GetAllAsync();
            var result = features.Select(_mapper.Map<Feature, KeyValuePairResource>);
            return Ok(result);
        }
    }
}
