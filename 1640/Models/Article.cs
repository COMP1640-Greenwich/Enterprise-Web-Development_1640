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
        //public int FacultyId { get; set; }
        //[ForeignKey("FacultyId")]
        //[ValidateNever]
        //public Faculty Faculty { get; set; }
        //[ValidateNever]
        [ValidateNever]
        public string? ImageUrl { get; set; }
        [ValidateNever]
        public string? DocxUrl { get; set; }
        public bool IsBlogActive { get; set; }
        [ValidateNever]
        public int SemesterId { get; set; }
        [ForeignKey("SemesterId")]
        [ValidateNever]
        public Semester Semester { get; set; }

        [ValidateNever]
        public string? UserId { get; set; }
        [ValidateNever]
        public string? UserName { get; set; }
        [ValidateNever]
        public int? FacultyId { get; set; }
        [ValidateNever]
        public string? FacultyName { get; set; }
        //[ForeignKey("FacultyId")]
        //[ValidateNever]
        //public Faculty Faculty { get; set; }


        public enum StatusArticle
        {
            Approve,
            Reject,
            Pending
        }
        [Required]
        [ValidateNever]
        public StatusArticle Status { get; set; }
        

    }
}
