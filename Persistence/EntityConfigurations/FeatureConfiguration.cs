using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vega.Models;

namespace Vega.Persistence.EntityConfigurations
{
    public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}