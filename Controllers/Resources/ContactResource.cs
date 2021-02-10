using System.ComponentModel.DataAnnotations;

namespace Vega.Controllers.Resources
{
    public class ContactResource
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Phone { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }
    }
}