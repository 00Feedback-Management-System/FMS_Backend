namespace Feedback_System.DTO
{
    public class RatingResultDto
    {

        public int QuestionId { get; set; }
        public string Question { get; set; }
        public double AverageRating { get; set; }
        public int TotalResponses { get; set; }
    }
}
