using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feedback_System.Model
{
    public class FeedbackGroup
    {
        [Key]
       public int FeedbackGroupId {  get; set; }
        [ForeignKey("Feedback")]
        public int? FeedbackId { get; set; }
        [ForeignKey("Groups")]
        public int? GroupId { get; set; }
        [ForeignKey("Staff")]
        public int? StaffId { get; set; }

        public Feedback Feedback { get; set; }

        public Groups Groups { get; set; }

        public Staff Staff { get; set; }
    }
}
