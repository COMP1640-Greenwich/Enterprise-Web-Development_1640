using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        public string Campus { get; set; }

        [ValidateNever] public string Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? FacultyId { get; set; } = null;
        public Faculty Faculty { get; set; }
    }
}
