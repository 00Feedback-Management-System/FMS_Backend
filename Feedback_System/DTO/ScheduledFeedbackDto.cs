namespace Feedback_System.DTO
{
    public class ScheduledFeedbackDto
    {
        public int FeedbackId { get; set; }
        public string CourseName { get; set; }
        public string ModuleName { get; set; }
        public string FeedbackTypeName { get; set; }
        public string StaffName { get; set; }
        public string GroupName { get; set; }
        public int Session { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}
