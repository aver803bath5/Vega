using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vega.Core.Domain;

namespace Vega.Persistence.EntityConfigurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.OwnsOne(v => v.Contact, nb =>
            {
                nb.Property(c => c.Email).HasColumnName("ContactEmail")
                    .HasMaxLength(255);

                nb.Property(c => c.Name).HasColumnName("ContactName")
                    .IsRequired()
                    .HasMaxLength(255);

                nb.Property(c => c.Phone).HasColumnName("ContactPhone")
                    .IsRequired()
                    .HasMaxLength(255);
            });
            builder.Navigation(v => v.Contact).IsRequired();
        }
    }
}