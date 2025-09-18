namespace Feedback_System.DTO
{
    public class ScheduledFeedbackDto
    {
        public int FeedbackId { get; set; }
        public int feedback_type_id { get; set; }//added for rating it can be caculated by using feedback type
        public string CourseName { get; set; }
        public string ModuleName { get; set; }
        public string FeedbackTypeName { get; set; }
        public string StaffName { get; set; }
        public string GroupName { get; set; }
        public int Session { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public double Rating { get; set; }
    }
}
