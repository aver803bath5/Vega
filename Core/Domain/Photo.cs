namespace Vega.Core.Domain
{
    public class Photo
    {
        public int Id { get; set; }

        public string FilePath { get; set; }
        public string RequestPath { get; set; }

        public Vehicle Vehicle { get; set; }
        public int VehicleId { get; set; }
    }
}