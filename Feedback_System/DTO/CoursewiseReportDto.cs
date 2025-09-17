namespace Feedback_System.DTO
{
    public class CoursewiseReportDto
    {

        public string CourseName { get; set; }
        public string ModuleName { get; set; }
        public List<string> FeedbackTypeNames { get; set; } = new List<string>();
    }
}
