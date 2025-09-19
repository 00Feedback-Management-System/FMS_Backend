using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.Model
{
    public class Modules
    {
        [Key]
        public int module_id { get; set; }
        [ForeignKey("Course")]
        public int? course_id { get; set; }

        public string module_name { get; set; }
        public int duration { get; set; }

        public Course? Course { get; set; }
    }
}
