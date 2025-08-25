using System.ComponentModel.DataAnnotations;

namespace Feedback_System.Model
{
    public class Modules
    {
        [Key]
        public int module_id { get; set; }

        public string module_name { get; set; }

        public int duration { get; set; }
    }
}
