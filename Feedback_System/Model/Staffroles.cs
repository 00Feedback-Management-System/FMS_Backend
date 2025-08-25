using System.ComponentModel.DataAnnotations;

namespace Feedback_System.Model
{
    public class Staffroles
    {
        [Key]
        public int staffrole_id { get; set; }

        public string staffrole_name { get; set; }
    }
}
