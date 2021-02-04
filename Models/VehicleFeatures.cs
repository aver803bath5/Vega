namespace Vega.Models
{
    public class VehicleFeatures
    {
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public int FeatureId { get; set; }
        public Feature Feature { get; set; }
    }
}