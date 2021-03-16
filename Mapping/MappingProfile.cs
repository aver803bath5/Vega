using System;
using System.Linq;
using AutoMapper;
using Vega.Controllers.Resources;
using Vega.Core.Domain;

namespace Vega.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain model to resource
            CreateMap<Make, MakeResource>();
            CreateMap<Make, KeyValuePairResource>();
            CreateMap<Model, KeyValuePairResource>();
            CreateMap<Feature, KeyValuePairResource>();
            CreateMap<Photo, PhotoResource>();
            CreateMap<Contact, ContactResource>();
            CreateMap<Vehicle, VehicleResource>()
                .ForMember(vr => vr.Make, opt => opt.MapFrom(v => v.Model.Make))
                .ForMember(vr => vr.Features,
                    opt => opt.MapFrom(v =>
                        v.Features.Select(vf => new KeyValuePairResource() {Id = vf.FeatureId, Name = vf.Feature.Name})));
            
            // Resource to domain model
            CreateMap<PhotoResource, Photo>();
            CreateMap<ContactResource, Contact>();
            CreateMap<SaveVehicleResource, Vehicle>()
                .ForMember(v => v.Id, opt => opt.Ignore())
                .ForMember(v => v.Features, opt => opt.Ignore())
                .AfterMap((vr, v) =>
                {
                    // Remove features
                    var removedFeatures = v.Features
                        .Where(vf => !vr.Features.Contains(vf.FeatureId))
                        .ToList();
                    foreach (var f in removedFeatures)
                        v.Features.Remove(f);

                    // // Add features
                    var addedFeatures = vr.Features
                        // Determine if vehicle features in VehicleFeatures table has same Feature Ids of features in VehicleResource.
                        .Where(id => v.Features.All(vf => vf.FeatureId != id))
                        .Select(id => new VehicleFeature() {FeatureId = id});
                    foreach (var addedFeature in addedFeatures)
                        v.Features.Add(addedFeature);
                });
        }
    }
}