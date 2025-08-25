using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.Model
{
    public class CourseStudent
    {
        [Key]

        public int course_student_id { get; set; }

        [ForeignKey("Course")]

        public int course_id { get; set; }

        [ForeignKey("Student")]

        public int student_rollno { get; set; }


        // class defined for foreign key relation

        public Course Course { get; set; }

        public Student Student { get; set; }
    }
}
