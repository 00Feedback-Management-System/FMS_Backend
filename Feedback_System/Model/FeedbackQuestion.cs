using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.Model
{
    public class FeedbackQuestion
    {
        [Key]

        public int question_id { get; set; }

        public string question { get; set; }

        public string question_type { get; set; }

        [ForeignKey("FeedbackType")]

        public int feedback_type_id { get; set; }

        // class defined for foreign key relation

        public FeedbackType FeedbackType { get; set; }
    }
}
