using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vega.Models;

namespace Vega.EntityConfigurations
{
    public class MakeConfiguration : IEntityTypeConfiguration<Make>
    {
        public void Configure(EntityTypeBuilder<Make> builder)
        {
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}
