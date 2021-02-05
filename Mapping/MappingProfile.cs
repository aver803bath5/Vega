using System.Linq;
using AutoMapper;
using Vega.Controllers.Resources;
using Vega.Models;

namespace Vega.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Make, MakeResource>();
            CreateMap<Model, ModelResource>();
            CreateMap<Feature, FeatureResource>();
            CreateMap<Vehicle, VehicleResource>()
                .ForMember(vr => vr.Make, opt => opt.MapFrom(v => v.Model.Make))
                .ForMember(vr => vr.Features,
                    opt => opt.MapFrom(v =>
                        v.Features.Select(vf => new FeatureResource {Id = vf.FeatureId, Name = vf.Feature.Name})));
            CreateMap<SaveVehicleResource, Vehicle>()
                .ForMember(v => v.Id, opt => opt.Ignore())
                .ForMember(v => v.Features, opt => opt.Ignore())
                .AfterMap((vr, v) =>
                {
                    var removedFeatures = v.Features
                        .Where(vf => !vr.Features.Contains(vf.FeatureId)).ToList();
                    foreach (var removedFeature in removedFeatures)
                        v.Features.Remove(removedFeature);

                    var addedFeatures = vr.Features
                        // Determine if vehicle features in VehicleFeatures table has same Feature Ids of features in VehicleResource.
                        .Where(id => v.Features.All(vf => vf.FeatureId == id))
                        .Select(id => new VehicleFeature() {FeatureId = id})
                        .ToList();
                    foreach (var addedFeature in addedFeatures)
                        v.Features.Add(addedFeature);
                });
        }
    }
}