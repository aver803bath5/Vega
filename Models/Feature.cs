using System.Collections.Generic;

namespace Vega.Models
{
    public class Feature
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }

        public Feature()
        {
            Vehicles = new List<Vehicle>();
        }
    }
}