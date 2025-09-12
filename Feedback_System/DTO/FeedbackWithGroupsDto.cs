namespace Feedback_System.DTO
{
    public class FeedbackWithGroupsDto
    {
        public int FeedbackId { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? FeedbackTypeId { get; set; }
        public int Session { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        // Groups + Staff under this feedback
        public List<FeedbackGroupDto> FeedbackGroups { get; set; }
    }
}
