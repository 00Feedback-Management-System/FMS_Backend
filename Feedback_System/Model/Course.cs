using System.ComponentModel.DataAnnotations;

namespace Feedback_System.Model
{
    public class Course
    {
        [Key]
        public int course_id { get; set; }

        public string course_name { get; set; }

        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }

        public int duration { get; set; }

        public string course_type { get; set; }
    }
}
