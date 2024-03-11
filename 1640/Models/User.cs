using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1640.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            CreatedAt = DateTime.UtcNow;
        }

        [Required]
        public string FullName { get; set; }
        [Required]
        public string HomeAddress { get; set; }

        [NotMapped] public string Role { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
