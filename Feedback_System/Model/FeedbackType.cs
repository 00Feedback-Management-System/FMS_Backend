using System.ComponentModel.DataAnnotations;

namespace Feedback_System.Model
{
    public class FeedbackType
    {
        [Key]

        public int feedback_type_id { get; set; }

        public string feedback_type_title { get; set; }

        public string feedback_type_description { get; set; }

        public Boolean is_module { get; set; }

        public string group { get; set; }

        public Boolean is_staff { get; set; }

        public Boolean is_session { get; set; }

        public Boolean behaviour { get; set; }
    }
}
