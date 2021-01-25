using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vega.Models
{
    public class Model
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Make> Makes { get; set; }

        public Model()
        {
            Makes = new List<Make>();
        }
    }
}
