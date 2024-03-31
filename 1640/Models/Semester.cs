using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _1640.Models
{
    public class Semester
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Start date and time cannot be empty")]
        //validate:Must be greater than current date
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date and time cannot be empty")]
        //validate:must be greater than StartDate
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
        [ValidateNever]
        public int FacultyId { get; set; }    // khóa ngoại tham chiếu tới tên của Faculity 
        [ForeignKey("FacultyId")]
        [ValidateNever]
        public virtual Faculty Faculty { get; set; }
        [ValidateNever]
        public string Status { get {
                if (DateTime.Now > EndDate)
                {
                    return "Closing";
                }
                else
                {
                    return "Opening";
                }
            }
        }

    }
}
