using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.Model
{
    public class FeedbackAnswer
    {
        [Key]

        public int answer_id { get; set; }

        [ForeignKey("FeedbackQuestion")]

        public int? question_id { get; set; }

        public string answer { get; set; }

        [ForeignKey("FeedbackSubmit")]

        public int? feedback_submit_id { get; set; }

        // class defined for foreign key relation

        public FeedbackQuestion FeedbackQuestion { get; set; }

        public FeedbackSubmit FeedbackSubmit { get; set; }
    }
}
