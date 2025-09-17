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
                    feedback_group_id = feedbackSubmitDto.feedbackGroupId,
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

        //[HttpGet("GetOverallRating/{feedbackTypeId}")]
        //public async Task<ActionResult<object>> GetOverallRating(int feedbackTypeId)
        //{
        //    try
        //    {

        //        var validQuestionIds = await _db.FeedbackQuestions
        //            .Where(q => q.feedback_type_id == feedbackTypeId
        //                        && (q.question_type == "rating" || q.question_type == "mcq"))
        //            .Select(q => new { q.question_id, q.question_type })
        //            .ToListAsync();

        //        if (!validQuestionIds.Any())
        //        {
        //            return NotFound(new { message = "No rating or mcq questions found for this feedback type" });
        //        }

        //        var questionTypeDict = validQuestionIds.ToDictionary(q => q.question_id, q => q.question_type);

               
        //        var answers = await _db.FeedbackAnswers
        //            .Where(a => questionTypeDict.Keys.Contains(a.question_id ?? 0) && a.answer != null)
        //            .Select(a => new { a.question_id, a.answer })
        //            .ToListAsync();

        //        var numericAnswers = new List<int>();

               
        //        foreach (var ans in answers)
        //        {
        //            string qType = questionTypeDict[ans.question_id ?? 0];

        //            if (qType == "rating")
        //            {
        //                if (int.TryParse(ans.answer, out int ratingVal))
        //                {
        //                    numericAnswers.Add(ratingVal);
        //                }
        //            }
        //            else if (qType == "mcq")
        //            {

        //                var mapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        //        {
        //            { "Excellent", 5 },
        //            { "Good", 4 },
        //            { "Average", 3 },
        //            { "Poor", 2 },
        //            { "Yes", 5 },
        //            { "No", 1 }
        //        };

        //                if (mapping.TryGetValue(ans.answer.Trim(), out int mcqVal))
        //                {
        //                    numericAnswers.Add(mcqVal);
        //                }
        //            }
        //        }

        //        if (!numericAnswers.Any())
        //        {
        //            return NotFound(new { message = "No valid numeric answers found for rating/mcq questions" });
        //        }

               
        //        double overallRating = Math.Round(numericAnswers.Average(), 2);

        //        return Ok(new
        //        {
        //            feedbackTypeId,
        //            overallRating,
        //            totalResponses = numericAnswers.Count
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        //    }
        //}

    }
}
