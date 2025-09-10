using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.DTO
{
    public class FeedbackCreateDto
    {
        public int FeedbackId { get; set; }

        public int course_id { get; set; }
        public string? course_name { get; set; }

        public int module_id { get; set; }
        public string? module_name { get; set; }

        public int feedback_type_id { get; set; }
        public string? feedback_type_title { get; set; }

        public int session { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string status { get; set; }

        // Single staff (if no groups)
        public int? staff_id { get; set; }

        // Multiple groups
        public List<FeedbackGroupDto> Groups { get; set; } = new List<FeedbackGroupDto>();
    }

    public class FeedbackGroupDto
    {
        public int? group_id { get; set; }     
        public int staff_id { get; set; }       
    }

}
