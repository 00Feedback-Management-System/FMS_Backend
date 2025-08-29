namespace Feedback_System.DTO
{
    public class FeedbackTypeDto
    {
        public int feedback_type_id { get; set; }
        public String feedback_type_title { get; set; }

        public String feedback_type_description { get; set; }
        public Boolean is_module { get; set; }
        public String group { get; set; }

        public Boolean is_staff { get; set; }
        public Boolean is_session { get; set; }
        public Boolean behaviour { get; set; }

    }
}
