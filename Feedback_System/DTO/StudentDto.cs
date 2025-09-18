namespace Feedback_System.DTO
{
    public class StudentDto
    {
        public int? student_rollno { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public int group_id { get; set; }

        public string profile_image { get; set; }
        public DateTime? login_time { get; set; }
    }
}
