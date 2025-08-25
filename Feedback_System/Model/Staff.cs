using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.Model
{
    public class Staff
    {
        [Key]
        public int staff_id { get; set; }

        [ForeignKey("Staffroles")]
        public int staffrole_id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        [ForeignKey("Groups")]

        public int group_id { get; set; }

        public string profile_image { get; set; }

        public DateTime login_time { get; set; }

        // class defined for foreign key relation

        public Staffroles Staffroles { get; set; }

        public Groups Groups { get; set; }
    }
}
