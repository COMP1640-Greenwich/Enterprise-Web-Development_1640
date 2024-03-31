using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1640.Models.VM
{
    public class ArticleVM
    {
        public Article Article { get; set; }
        [ValidateNever]
        
        public IEnumerable<SelectListItem> Semesters { get; set; }
        [ValidateNever]
        public int? FacultyId { get; set; }
        [ValidateNever]
        public string? UserId { get; set; }
        [ValidateNever]
        public string? UserName { get; set; }
        [ValidateNever]
        public string? FacultyName { get; set; }

    }
}
