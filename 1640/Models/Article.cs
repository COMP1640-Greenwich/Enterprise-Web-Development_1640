using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace _1640.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public int FaculityId { get; set; }    // khóa ngoại tham chiếu tới tên của Faculty 
        //[ForeignKey("FaculityId")]
        //[ValidateNever]
        //public virtual Faculity Faculity { get; set; }
        //[ValidateNever]
        //public int SemesterId { get; set; } // khóa ngoại tham chiếu tới tên của Semester 
        //[ForeignKey("SemesterId")]
        //public virtual Semester Semester { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }
        [ValidateNever]
        public string? DocxUrl { get; set; }
        public bool IsBlogActive { get; set; }
        public enum StatusArticle
        {
            Approve,
            Reject,
            Pending
        }

        [Required]
        public StatusArticle Status { get; set; }
    }
}
