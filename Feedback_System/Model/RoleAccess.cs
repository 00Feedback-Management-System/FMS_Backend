using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.Model
{
    public class RoleAccess
    {
        [Key]

        public int role_access_id { get; set; }

        [ForeignKey("Staffroles")]
        public int staffrole_id { get; set; }

        [ForeignKey("Students")]

        public int student_rollno { get; set; }

        public string route { get; set; }

        public string component { get; set; }

        // class defined for foreign key relation

        public Staffroles Staffroles { get; set; }

        public Student Students { get; set; }
    }
}
