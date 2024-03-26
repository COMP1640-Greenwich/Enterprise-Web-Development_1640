using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _1640.Models.VM
{
    public class CommentVM
    {
        public Comment Comment{ get; set; }
        public int CommentId { get; set; }
        public int ArticleId { get; set; }
        public string Text { get; set; }
        public DateTime CommentOn { get; set; }
    }
}
