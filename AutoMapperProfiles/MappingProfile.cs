using AutoMapper;
using Vega.Dtos;
using Vega.Models;

namespace Vega.AutoMapperProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Make, MakeDto>();
            CreateMap<Model, ModelDto>();
        }
    }
}
