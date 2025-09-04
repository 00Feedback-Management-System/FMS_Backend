using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.DTO
{
    public class FeedbackDto
    {
        //[Column("feedback_id")]
        public int feedback_id { get; set; }
        public int course_id { get; set; }
        public int module_id { get; set; }
        public int feedback_type_id { get; set; }
        public int staff_id { get; set; }
        public int session { get; set; }

        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }

        public string status { get; set; }

        public List<GroupDto> Groups { get; set; }
        // public List<CourseDTO> course { get; set; }
        public string course_name { get; set; }

        public string module_name { get; set; }

        public string feedback_type_title { get; set; }
        //  public FeedbackType FeedbackType { get; set; }

        public string first_name { get; set; }

    }
}
