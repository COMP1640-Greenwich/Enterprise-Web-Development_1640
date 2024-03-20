using System.ComponentModel.DataAnnotations;

namespace _1640.Models
{
    public class Faculity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
