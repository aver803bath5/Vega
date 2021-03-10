using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vega.Core.Domain;

namespace Vega.Persistence.EntityConfigurations
{
    public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
    {
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder.Property(p => p.FilePath).HasMaxLength(255).IsRequired();
            builder.Property(p => p.RequestPath).HasMaxLength(255).IsRequired();
        }
    }
}