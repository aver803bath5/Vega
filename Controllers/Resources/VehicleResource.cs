using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Vega.Controllers.Resources
{
    public class VehicleResource
    {
        public int Id { get; set; }

        public KeyValuePairResource Model { get; set; }

        public KeyValuePairResource Make { get; set; }

        public ICollection<KeyValuePairResource> Features { get; set; }

        [Required] public ContactResource Contact { get; set; }

        public bool IsRegistered { get; set; }

        public VehicleResource()
        {
            Features = new Collection<KeyValuePairResource>();
        }
    }
}