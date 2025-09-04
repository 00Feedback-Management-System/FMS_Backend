using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feedback_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackTypeController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public FeedbackTypeController(ApplicationDBContext db)
        {
            _db = db;
        }

        // GET: api/FeedbackType/GetFeedbackType
        [Route("GetFeedbackType")]
        [HttpGet]
        public ActionResult<IEnumerable<FeedbackTypeDto>> GetFeedbackTypes()
        {
            var types = _db.FeedbackType
                .Select(f => new FeedbackTypeDto
                {
                    feedback_type_id = f.feedback_type_id,
                    feedback_type_title = f.feedback_type_title,
                    feedback_type_description = f.feedback_type_description,
                    is_module = f.is_module,
                    group = f.group,
                    is_staff = f.is_staff,
                    is_session = f.is_session,
                    behaviour = f.behaviour
                })
                .ToList();

            return Ok(types);
        }

        // GET: api/FeedbackType/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackType(int id)
        {
            var feedbackType = await _db.FeedbackType
                .Include(ft => ft.FeedbackQuestions)
                .FirstOrDefaultAsync(ft => ft.feedback_type_id == id);

            if (feedbackType == null)
                return NotFound();

            var result = new
            {
                FeedbackTypeId = feedbackType.feedback_type_id,
                FeedbackTypeTitle = feedbackType.feedback_type_title,
                FeedbackTypeDescription = feedbackType.feedback_type_description,
                IsModule = feedbackType.is_module,
                Group = feedbackType.group,
                IsStaff = feedbackType.is_staff,
                IsSession = feedbackType.is_session,
                Behaviour = feedbackType.behaviour,
                Questions = feedbackType.FeedbackQuestions.Select(q => new
                {
                    QuestionId = q.question_id,
                    Question = q.question,
                    QuestionType = q.question_type
                }).ToList()
            };

            return Ok(result);
        }

        // POST: api/FeedbackType/CreateFeedbackType
        [Route("CreateFeedbackType")]
        [HttpPost]
        public async Task<IActionResult> CreateFeedbackType([FromBody] CreateFeedbackTypeDto dto)
        {
            if (dto == null) return BadRequest("Invalid data");

            var feedbackType = new FeedbackType
            {
                feedback_type_title = dto.FeedbackTypeTitle,
                feedback_type_description = dto.FeedbackTypeDescription,
                is_module = dto.IsModule,
                group = dto.Group,
                is_staff = dto.IsStaff,
                is_session = dto.IsSession,
                behaviour = dto.Behaviour
            };

            _db.FeedbackType.Add(feedbackType);
            await _db.SaveChangesAsync();

            if (dto.Questions != null && dto.Questions.Any())
            {
                var questions = dto.Questions.Select(q => new FeedbackQuestion
                {
                    question = q.Question,
                    question_type = q.QuestionType,
                    feedback_type_id = feedbackType.feedback_type_id
                }).ToList();

                _db.FeedbackQuestions.AddRange(questions);
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                Message = "FeedbackType created successfully",
                FeedbackTypeId = feedbackType.feedback_type_id
            });
        }

        // PUT: api/FeedbackType/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedbackType(int id, [FromBody] CreateFeedbackTypeDto dto)
        {
            if (dto == null) return BadRequest("Invalid data");

            var feedbackType = await _db.FeedbackType
                .Include(ft => ft.FeedbackQuestions)
                .FirstOrDefaultAsync(ft => ft.feedback_type_id == id);

            if (feedbackType == null) return NotFound();

            // Update main fields
            feedbackType.feedback_type_title = dto.FeedbackTypeTitle;
            feedbackType.feedback_type_description = dto.FeedbackTypeDescription;
            feedbackType.is_module = dto.IsModule;
            feedbackType.group = dto.Group;
            feedbackType.is_staff = dto.IsStaff;
            feedbackType.is_session = dto.IsSession;
            feedbackType.behaviour = dto.Behaviour;

            // Replace questions: delete old and add new
            _db.FeedbackQuestions.RemoveRange(feedbackType.FeedbackQuestions);

            if (dto.Questions != null && dto.Questions.Any())
            {
                feedbackType.FeedbackQuestions = dto.Questions.Select(q => new FeedbackQuestion
                {
                    question = q.Question,
                    question_type = q.QuestionType,
                    feedback_type_id = feedbackType.feedback_type_id
                }).ToList();
            }

            await _db.SaveChangesAsync();

            return Ok(new { Message = "FeedbackType updated successfully" });
        }

        // DELETE: api/FeedbackType/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedbackType(int id)
        {
            var feedbackType = await _db.FeedbackType
                .Include(ft => ft.FeedbackQuestions)
                .FirstOrDefaultAsync(ft => ft.feedback_type_id == id);

            if (feedbackType == null)
                return NotFound(new { Message = "FeedbackType not found" });

            if (feedbackType.FeedbackQuestions != null && feedbackType.FeedbackQuestions.Any())
            {
                _db.FeedbackQuestions.RemoveRange(feedbackType.FeedbackQuestions);
            }

            _db.FeedbackType.Remove(feedbackType);
            await _db.SaveChangesAsync();

            return Ok(new { Message = "FeedbackType deleted successfully" });
        }
    }
}
