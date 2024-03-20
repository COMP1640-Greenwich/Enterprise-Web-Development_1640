using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1640.Models
{
    public class PublicCoordinator
    {
        [Key]
        public int Id { get; set; }
        [ValidateNever]
        public string ContributionName { get; set; } // khóa ngoại tham chiếu tới title trong Article
        //[ForeignKey("ContributionName")]
        //[Validation]
        //public Ideal Ideal { get; set; }
        [ValidateNever]
        public string StudentName { get; set; } // khóa ngoại tham chiếu tới name trong Student
        //[ForeignKey("StudentName")]
        //[Validation]
        //public Student Student { get; set; }
        // Action button cho phép Coordinator đẩy bài viết đó lên home page or not
    }
}
