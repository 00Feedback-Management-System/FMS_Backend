namespace Feedback_System.DTO
{
    public class CreateFeedbackTypeDto
    {
        public string FeedbackTypeTitle { get; set; }
        public string FeedbackTypeDescription { get; set; }
        public bool IsModule { get; set; }
        public string Group { get; set; }
        public bool IsStaff { get; set; }
        public bool IsSession { get; set; }
        public bool Behaviour { get; set; }

        public List<FeedbackQuestionDto> Questions { get; set; }
    }
}
