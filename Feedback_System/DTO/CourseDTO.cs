namespace Feedback_System.DTO
{
    public class CourseDTO
    {
        public int course_id { get; set; }
        public string course_name { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public int duration { get; set; }
        public string course_type { get; set; }
    }
}
