using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feedback_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionAnswerController : ControllerBase
    {
        public readonly ApplicationDBContext _db;

        public QuestionAnswerController(ApplicationDBContext dbContext)
        {
            _db = dbContext;
        }

        [HttpGet("GetAllQuestions/{feedbackTypeId}")]
        public async Task<ActionResult<IEnumerable<FeedbackQuestion>>> GetAllQuestions(int feedbackTypeId)
        {
            try
            {
               if(_db.FeedbackQuestions == null)
               {
                    return NotFound(new { message = "No questions found" });
               }

               var questions = await _db.FeedbackQuestions
                                        .Where(q => q.feedback_type_id == feedbackTypeId)
                                        .ToListAsync();

                if(questions == null || questions.Count == 0)
                {
                    return NotFound(new { message = $"No questions found for the specified feedback type ID: {feedbackTypeId}" });
                }

                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("SubmitFeedbackAnswers")]
        public async Task<IActionResult> SubmitFeedbackAnswers([FromBody] FeedbackSubmitDto feedbackSubmitDto)
        {
            if(feedbackSubmitDto == null || feedbackSubmitDto.answers == null || feedbackSubmitDto.answers.Count == 0)
            {
                return BadRequest(new { message = "Invalid feedback submission data" });
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var feedbackSubmit = new FeedbackSubmit
                {
                    student_rollno = feedbackSubmitDto.studentId,
                    feedback_id = feedbackSubmitDto.feedbackId,
                    submited_at = DateTime.UtcNow
                };

                _db.FeedbackSubmits.Add(feedbackSubmit);
                await _db.SaveChangesAsync();

                foreach(var answer in feedbackSubmitDto.answers)
                {
                    var feedbackAnswer = new FeedbackAnswer
                    {
                        question_id = int.Parse(answer.Key),
                        answer = answer.Value?.ToString(),
                        feedback_submit_id = feedbackSubmit.feedback_submit_id
                    };
                    _db.FeedbackAnswers.Add(feedbackAnswer);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Feedback submitted successfully" });
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error submitting feedback: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return StatusCode(500, new { message = "Failed to submit feedback answers", error = ex.Message });
            }
        }
    }
}
