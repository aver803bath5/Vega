using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vega.Models;

namespace Vega.EntityConfigurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.Property(v => v.ContactName)
                .IsRequired()
                .HasMaxLength(255);
            
            builder.Property(v => v.ContactPhone)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(v => v.ContactEmail)
                .HasMaxLength(255);
        }
    }
}