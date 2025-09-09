using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.DTO
{
    public class FeedbackCreateDto
    {
        public int FeedbackId { get; set; }
        public int course_id { get; set; }
        public int module_id { get; set; }
        public int feedback_type_id { get; set; }
        public int session { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string status { get; set; }
       // public ICollection<FeedbackGroupDto> Groups { get; set; }

        public List<FeedbackGroupDto> Groups { get; set; }
    }

    public class FeedbackGroupDto
    {
        public string group_name { get; set; }
        public int staff_id { get; set; }
    }

}
