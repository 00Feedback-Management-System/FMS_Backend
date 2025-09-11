using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Feedback_System.Model
{
    public class CourseGroup
    {
        [Key]
        public int course_group_id { get; set; }

        [ForeignKey("Course")]

        public int? course_id { get; set; }

        [ForeignKey("Groups")]

        public int? group_id { get; set; }

        // class defined for foreign key relation

        public Course Course { get; set; }

        public Groups Groups { get; set; }


    }
}
