namespace Feedback_System.DTO
{
    public class FeedbackAnswerDto
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
    }
}
