using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Vega.Controllers.Resources
{
    public class SaveVehicleResource
    {
        public int Id { get; set; }

        public int ModelId { get; set; }

        [Required] public ContactResource Contact { get; set; }

        public bool IsRegistered { get; set; }

        public ICollection<int> Features { get; set; }

        public DateTime LastUpdate { get; set; }

        public SaveVehicleResource()
        {
            Features = new Collection<int>();
        }
    }
}