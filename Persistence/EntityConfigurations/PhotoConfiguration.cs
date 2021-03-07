using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vega.Core.Domain;

namespace Vega.Persistence.EntityConfigurations
{
    public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
    {
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder.Property(p => p.FileName).HasMaxLength(255);
        }
    }
}