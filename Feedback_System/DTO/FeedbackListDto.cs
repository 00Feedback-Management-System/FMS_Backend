using Feedback_System.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.DTO
{

    public class FeedbackListDto
    {
        public int FeedbackId { get; set; }
        public int course_id { get; set; }
        public int module_id { get; set; }
        public int feedback_type_id { get; set; }
        public int session { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string status { get; set; }
    }

}

