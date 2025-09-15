namespace Feedback_System.DTO
{
    public class SubmittedFeedbackDetailsDto
    {
        public int FeedbackGroupId { get; set; }
        public string FeedbackTypeName { get; set; }
        public string CourseName { get; set; }
        public string ModuleName { get; set; }
        public string StaffName { get; set; }
        public int Session { get; set; }
        public List<FeedbackAnswerDto> Answers { get; set; }
    }
}
