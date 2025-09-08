using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feedback_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDBContext _db;

        public FeedbackController(ApplicationDBContext db)
        {
            _db = db;
        }

        [Route("GetFeedback")]
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<FeedbackDto>> GetFeedback()
        {
            var feedbacks = _db.Feedback
                .Include(f => f.Course)
                .Include(f => f.Module)
                .Include(f => f.FeedbackType)
                .Include(f => f.Staff)
                .Select(f => new FeedbackDto
                {
                    feedback_id = f.FeedbackId,
                    course_id = (int)f.course_id,
                    module_id = (int)f.module_id,
                    feedback_type_id = (int)f.feedback_type_id,
                    staff_id = (int)f.staff_id,
                    session = f.session,
                    start_date = f.start_date,
                    end_date = f.end_date,
                    status = f.status,


                    course_name = f.Course.course_name,
                    module_name = f.Module.module_name,
                    feedback_type_title = f.FeedbackType.feedback_type_title,
                    first_name = f.Staff.first_name
                })
                .ToList();

            return Ok(feedbacks);

        }

        [Route("CreateFeedback")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public ActionResult<FeedbackDto> CreateFeedback([FromBody] FeedbackDto dto)
        {
            if (dto == null)
                return BadRequest("Feedback  data is required.");

            var feedback = new Feedback
            {
                FeedbackId = dto.feedback_id,
                course_id = dto.course_id,
                module_id = dto.module_id,
                feedback_type_id = dto.feedback_type_id,
                staff_id = dto.staff_id,
                session = dto.session,
                start_date = dto.start_date,
                end_date = dto.end_date,
                status = dto.status ?? "Pending",

            };

            _db.Feedback.Add(feedback);
            _db.SaveChanges();

            if (dto.Groups != null && dto.Groups.Any())
            {
                var groups = dto.Groups.Select(q => new Groups
                {
                    group_id = q.group_id,
                    group_name = q.group_name,
                    group_count = q.group_count,
                }).ToList();

                _db.Groups.AddRange(groups);
                _db.SaveChanges();
            }

            dto.feedback_id = feedback.FeedbackId;


            return CreatedAtAction(nameof(GetFeedback),
               new { id = feedback.FeedbackId },
                dto);
        }

        [HttpDelete("DeleteFeedback/{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteFeedback(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid feedback type ID.");

            var feedback = _db.Feedback.FirstOrDefault(f => f.FeedbackId == id);
            if (feedback == null)
                return NotFound($"Feedback with ID '{id}' not found.");

            _db.Feedback.Remove(feedback);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("UpdateFeedback/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateFeedback(int id, [FromBody] FeedbackDto dto)
        {
            if (dto == null)
                return BadRequest("Feedback data is required.");

            var feedback = _db.Feedback.FirstOrDefault(f => f.FeedbackId == id);
            if (feedback == null)
                return NotFound($"Feedback with ID '{id}' not found.");

           
            feedback.course_id = dto.course_id;
            feedback.module_id = dto.module_id;
            feedback.feedback_type_id = dto.feedback_type_id;
            feedback.staff_id = dto.staff_id;
            feedback.session = dto.session;
            feedback.start_date = dto.start_date;
            feedback.end_date = dto.end_date;
            feedback.status = dto.status ?? feedback.status;

            _db.Feedback.Update(feedback);
            _db.SaveChanges();

            return Ok(new
            {
                Message = "Feedback updated successfully",
                UpdatedFeedback = dto
            });
        }
    }
}
