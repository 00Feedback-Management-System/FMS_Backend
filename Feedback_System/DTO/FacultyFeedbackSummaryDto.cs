namespace Feedback_System.DTO
{
    public class FacultyFeedbackSummaryDto
    {
        public string staff_name { get; set; }
        public string module_name { get; set; }

        public string course_name { get; set; }

        public string type_name { get; set; }

        public string date { get; set; }

        public int feedbackTypeId { get; set; }

        public int feedbackId { get; set; }

        public int feedbackGroupId { get; set; }


    }

}