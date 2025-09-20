namespace Feedback_System.DTO
{
    public class StudentCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? GroupId { get; set; }
        public int CourseId { get; set; }  //  we need course id for CourseStudent
    }

}
