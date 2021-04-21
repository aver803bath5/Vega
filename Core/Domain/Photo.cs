namespace Vega.Core.Domain
{
    public class Photo
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string Thumbnail { get; set; }

        public Vehicle Vehicle { get; set; }
        public int VehicleId { get; set; }
    }
}