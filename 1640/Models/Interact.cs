using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1640.Models
{
    public class Interact
    {
        [Key]
        public int Id { get; set; }

        public string StudentName { get; set; } // khóa ngoại tham chiếu tới tên của Student
        //[ForeignKey(StudentName)]
        //[Validation]
        //public Student Student { get; set; }
       
        public string StudentEmail { get; set; } //khóa ngoại tham chiếu tới tk gmail của Student
        //[ForeignKey(StudentEmail)]
        //[Validation]
        //public Student Student { get; set; }
        // Chat button giúp cho Coordinator có thể interact với students thông qua gmail

    }
}
