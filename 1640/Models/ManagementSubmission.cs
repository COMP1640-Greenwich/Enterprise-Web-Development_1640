using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1640.Models
{
    public class ManagementSubmission
    {
        [Key]
        public string Id { get; set; } 
        //public Student Student { get; set; }
        //[ForeignKey("StudentName")]
        //[Validation]
        public string StudentName { get; set; } // khóa ngoại tham chiếu tới name trong students
        public string Email { get; set; }
        public string Status { get; set; } // status ở đây là trạng thái accept or not
        public string Commemt {  get; set; } // comment ở trong bài viết
        // Edit Coordinator có thể edit bài viết
        

    }
}
