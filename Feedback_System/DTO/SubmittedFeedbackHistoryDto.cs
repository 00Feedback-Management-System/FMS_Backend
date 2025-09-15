namespace Feedback_System.DTO
{
    public class SubmittedFeedbackHistoryDto
    {
        public int FeedbackGroupId { get; set; }
        public int? FeedbackId { get; set; }
        public string FeedbackTypeName { get; set; }
        public string CourseName { get; set; }
        public string ModuleName { get; set; }
        public string StaffName { get; set; }
        public int Session { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int? FeedbackTypeId { get; set; }
    }
}
