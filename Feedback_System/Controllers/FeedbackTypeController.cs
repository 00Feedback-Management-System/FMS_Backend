using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet("ByGroup/{groupType}")]
        public async Task<IActionResult> GetFeedbackTypesByGroup(string groupType)
        {
            if (string.IsNullOrEmpty(groupType))
                return BadRequest("Group type is required.");

            // normalize input (in case user passes "Single"/"MULTIPLE")
            groupType = groupType.ToLower();

            var feedbackTypes = await _db.FeedbackType
                .Where(ft => ft.group.ToLower() == groupType)
                .Select(ft => new
                {
                    FeedbackTypeId = ft.feedback_type_id,
                    Title = ft.feedback_type_title,
                    Description = ft.feedback_type_description,
                    IsModule = ft.is_module,
                    Group = ft.group,
                    IsStaff = ft.is_staff,
                    IsSession = ft.is_session,
                    Behaviour = ft.behaviour
                })
                .ToListAsync();

            if (!feedbackTypes.Any())
                return NotFound($"No feedback types found for group '{groupType}'.");

            return Ok(feedbackTypes);
        }


        // POST: api/FeedbackType/CreateFeedbackType
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedbackType(int id)
        {
            var feedbackType = await _db.FeedbackType
                .Include(ft => ft.FeedbackQuestions)
                .FirstOrDefaultAsync(ft => ft.feedback_type_id == id);

            if (feedbackType == null)
                return NotFound(new { Message = "FeedbackType not found" });

           
            bool hasFeedbacks = await _db.Feedback.AnyAsync(f => f.feedback_type_id == id);
            if (hasFeedbacks)
            {
                return BadRequest(new { Message = "Cannot delete this FeedbackType because feedbacks already exist for it." });
            }

           
            if (feedbackType.FeedbackQuestions != null && feedbackType.FeedbackQuestions.Any())
            {
                _db.FeedbackQuestions.RemoveRange(feedbackType.FeedbackQuestions);
            }

            _db.FeedbackType.Remove(feedbackType);
            await _db.SaveChangesAsync();

            return Ok(new { Message = "FeedbackType deleted successfully" });
        }

        // GET: api/FeedbackType/CheckEditable/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("CheckEditable/{id}")]
        public async Task<IActionResult> CheckEditable(int id)
        {
            bool isScheduled = await _db.Feedback.AnyAsync(f => f.feedback_type_id == id);
            if (isScheduled)
                return BadRequest(new { message = "This Feedback Type is already scheduled and cannot be edited." });

            return Ok(new { message = "Editable" });
        }

    }
}
