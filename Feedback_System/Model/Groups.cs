using System.ComponentModel.DataAnnotations;

namespace Feedback_System.Model
{
    public class Groups
    {
        [Key]
        public int group_id { get; set; }

        public string group_name { get; set; }

        public int group_count { get; set; }
    }
}
