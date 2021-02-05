using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vega.Models
{
    public class Feature
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<VehicleFeatures> Vehicles { get; set; }

        public Feature()
        {
            Vehicles = new Collection<VehicleFeatures>();
        }
    }
}