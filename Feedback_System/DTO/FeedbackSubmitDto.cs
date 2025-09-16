namespace Feedback_System.DTO
{
    public class FeedbackSubmitDto
    {
        public int feedbackId { get; set; }
        public int feedbackGroupId { get; set; }
        public int studentId { get; set; }
        public Dictionary<string, object> answers { get; set; }
    }
}
