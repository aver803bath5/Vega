using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vega.Core.Domain
{
    public class Vehicle
    {
        public int Id { get; set; }

        public Model Model { get; set; }
        public int ModelId { get; set; }

        public ICollection<VehicleFeature> Features { get; set; }

        public Contact Contact { get; set; }

        public bool IsRegistered { get; set; }

        public DateTime LastUpdate { get; set; }

        public ICollection<Photo> Photos { get; set; }

        public Vehicle()
        {
            Features = new Collection<VehicleFeature>();
            Photos = new Collection<Photo>();
        }
    }
}