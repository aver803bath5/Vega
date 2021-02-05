using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vega.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        public Model Model { get; set; }
        public int ModelId { get; set; }

        public ICollection<VehicleFeature> Features { get; set; }

        public string ContactName { get; set; }

        public string ContactPhone { get; set; }

        public string ContactEmail { get; set; }

        public bool IsRegistered { get; set; }

        public DateTime LastUpdate { get; set; }

        public Vehicle()
        {
            Features = new Collection<VehicleFeature>();
        }
    }
}