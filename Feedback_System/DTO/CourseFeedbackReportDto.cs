namespace Feedback_System.DTO
{
    public class CourseFeedbackReportDto
    {
        public DateTime Date { get; set; }        // Max EndDate
        public string CourseName { get; set; }
        public string FeedbackTypeName { get; set; }
        public string Groups { get; set; }        // Single / Multiple
        public int Sessions { get; set; }         // Sum of sessions
        public double Rating { get; set; }        // Avg rating
    }
}
