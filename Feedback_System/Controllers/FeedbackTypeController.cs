using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feedback_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackTypeController : Controller
    {
        private readonly ApplicationDBContext _db;

        public FeedbackTypeController(ApplicationDBContext db)
        {
            _db = db;
        }


        // GET: api/FeedbackType

        [Route("GetFeedbackType")]
        [HttpGet]
        [ProducesResponseType(200)]
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

        [HttpGet("GetFeedbackById/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public ActionResult<FeedbackTypeDto> GetFeedbackType(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid feedback type ID.");

            var feedbackType = _db.FeedbackType.FirstOrDefault(f => f.feedback_type_id == id);
            if (feedbackType == null)
                return NotFound($"Feedback type with ID '{id}' not found.");

            var dto = new FeedbackTypeDto
            {
                feedback_type_id = feedbackType.feedback_type_id,
                feedback_type_title = feedbackType.feedback_type_title,
                feedback_type_description = feedbackType.feedback_type_description,
                is_module = feedbackType.is_module,
                group = feedbackType.group,
                is_staff = feedbackType.is_staff,
                is_session = feedbackType.is_session,
                behaviour = feedbackType.behaviour
            };

            return Ok(dto);
        }


        // POST: api/FeedbackType

        [Route("CreateFeedbackType")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateFeedbackType([FromBody] CreateFeedbackTypeDto dto)
        {
            if (dto == null) return BadRequest("Invalid data");

            // Create FeedbackType
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

            // Now insert related questions
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
        [HttpPut("UpdateFeedbackType")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateFeedbackType([FromBody] FeedbackTypeDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid question data.");
            var feedbackType = _db.FeedbackType.FirstOrDefault(f => f.feedback_type_id == dto.feedback_type_id);
            if (feedbackType == null)
                return NotFound($"Feedback type with ID '{dto.feedback_type_id}' not found.");

            feedbackType.feedback_type_title = dto.feedback_type_title;
            feedbackType.feedback_type_description = dto.feedback_type_description;
            feedbackType.is_module = dto.is_module;
            feedbackType.group = dto.group;
            feedbackType.is_staff = dto.is_staff;
            feedbackType.is_session = dto.is_session;
            feedbackType.behaviour = dto.behaviour;

            _db.FeedbackType.Update(feedbackType);
            _db.SaveChanges();
            return Ok(feedbackType);
        }

        // DELETE: api/FeedbackType/{id}

        [HttpDelete("DeleteFeedbackType/{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteFeedbackType(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid feedback type ID.");

            var feedbackType = _db.FeedbackType.FirstOrDefault(f => f.feedback_type_id == id);
            if (feedbackType == null)
                return NotFound($"Feedback type with ID '{id}' not found.");

            _db.FeedbackType.Remove(feedbackType);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
