using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1640.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        [ScaffoldColumn(false)]
        public DateTime CommentOn { get; set; }
        [ValidateNever]
        public int ArticleId { get; set; }
    }
}
