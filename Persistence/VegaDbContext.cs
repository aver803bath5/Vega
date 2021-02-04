using Microsoft.EntityFrameworkCore;
using Vega.EntityConfigurations;
using Vega.Models;

namespace Vega.Persistence
{
    public class VegaDbContext : DbContext
    {
        public DbSet<Model> Models { get; set; }
        public DbSet<Make> Makes { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        public VegaDbContext(DbContextOptions<VegaDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ModelConfiguration());
            modelBuilder.ApplyConfiguration(new MakeConfiguration());
            modelBuilder.ApplyConfiguration(new FeatureConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleConfiguration());

            modelBuilder.Entity<VehicleFeatures>().HasKey(vf => new {vf.VehicleId, vf.FeatureId});
        }
    }
}