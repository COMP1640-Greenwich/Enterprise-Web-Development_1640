using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _1640.Models.VM
{
    public class SemesterVM
    {
        public Semester Semester { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Faculities { get; set;}
        [ValidateNever]
        public IEnumerable<SelectListItem> Articles { get; set; }
       
        
    }
}
