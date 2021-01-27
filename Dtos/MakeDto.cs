using System.Collections.Generic;

namespace Vega.Dtos
{
    public class MakeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<ModelDto> Models { get; set; }
    }
}