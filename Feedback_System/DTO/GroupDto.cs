namespace Feedback_System.DTO
{
    public class GroupDto
    {
        public int group_id { get; set; }

        public string group_name { get; set; }

        public int group_count { get; set; }
        public int FeedbackId { get; internal set; }
    }
}
