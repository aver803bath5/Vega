using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vega.Models;

namespace Vega.Persistence.EntityConfigurations
{
    public class VehicleFeatureConfiguration : IEntityTypeConfiguration<VehicleFeature>
    {
        public void Configure(EntityTypeBuilder<VehicleFeature> builder)
        {
            builder.ToTable("VehicleFeatures");

            builder.HasKey(vf => new {vf.VehicleId, vf.FeatureId});
        }
    }
}