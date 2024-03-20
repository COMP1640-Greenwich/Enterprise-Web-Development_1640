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
        //public int FaculityId { get; set; }
        //[ForeignKey("FacultyId")]
        //[ValidateNever]
        //public Faculity Faculity { get; set; }
        //[ValidateNever]
        //public int SemesterId { get; set; }
        //[ForeignKey("SemesterId")]
        //[ValidateNever]
        //public Semester Semester { get; set; }
        public string? ImageUrl { get; set; }
        public string? DocxUrl { get; set; }
        public bool IsBlogActive { get; set; }
    }
}
