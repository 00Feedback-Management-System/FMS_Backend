using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Feedback_System.Model
{
    [Table("Feedback")]
    public class Feedback
    {

        [Key]
        [Column("feedback_id")]
        public int FeedbackId { get; set; } //mapped feedback_id with FeedbackId

        [ForeignKey("Course")]

        public int course_id { get; set; }

        [ForeignKey("Module")]

        public int module_id { get; set; }

        [ForeignKey("FeedbackType")]

        public int feedback_type_id { get; set; }

        [ForeignKey("Staff")]

        public int staff_id { get; set; }

        public int session { get; set; }

        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }

        public string status { get; set; }

        // class defined for foreign key relation

        public Course Course { get; set; }

        public Modules Module { get; set; }


        public FeedbackType FeedbackType { get; set; }

        public Staff Staff { get; set; }
    }
}
