namespace Feedback_System.DTO
{
    public class FeedbackDto
    {
        public int FeedbackGroupId { get; set; }
        public int FeedbackId { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? FeedbackTypeId { get; set; }
        public int Session { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        // If single staff
        public int? StaffId { get; set; }

        // If multiple groups
        public List<FeedbackGroupDto> FeedbackGroups { get; set; }
    }
}
