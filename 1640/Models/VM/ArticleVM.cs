using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _1640.Models.VM
{
    public class ArticleVM
    {
        public Article Article { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Semesters { get; set; }
    }
}
