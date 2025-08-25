using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.Model
{
    public class FeedbackSubmit
    {
        [Key]

        public int feedback_submit_id { get; set; }

        [ForeignKey("Students")]
        public int student_rollno { get; set; }

        [ForeignKey("Feedback")]

        public int feedback_id { get; set; }

        public DateTime submited_at { get; set; }


        // class defined for foreign key relation

        public Student Students { get; set; }

        public Feedback Feedback { get; set; }
    }
}
