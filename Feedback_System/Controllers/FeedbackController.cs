using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
        public ActionResult<IEnumerable<FeedbackListDto>> GetFeedback()
        {
            var types = _db.Feedback
                .Select(f => new FeedbackListDto
                {
                    FeedbackId = f.FeedbackId,
                    course_id = f.course_id,
                    module_id = f.module_id,
                    feedback_type_id = f.feedback_type_id,
                    session = f.session,
                    start_date = f.start_date,
                    end_date = f.end_date,
                    status = f.status
                })
                .ToList();

            return Ok(types);
        }
        [Route("CreateFeedback")]
        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateDto dto)
        {
            if (dto == null) return BadRequest("Invalid data");

            var feedback = new Feedback
            {
                FeedbackId = dto.FeedbackId,
                course_id = dto.course_id,
                module_id = dto.module_id,
                feedback_type_id = dto.feedback_type_id,
                session = dto.session,
                start_date = dto.start_date,
                end_date = dto.end_date,
                status = dto.status
            };

            _db.Feedback.Add(feedback);
            await _db.SaveChangesAsync();

            if (dto.Groups != null && dto.Groups.Any())
            {
                var groups = new List<FeedbackGroup>();

                foreach (var q in dto.Groups)
                {
                    // find the group id by name
                    var groupEntity = await _db.Groups
                        .FirstOrDefaultAsync(g => g.group_name == q.group_name);

                    if (groupEntity == null)
                    {
                        return BadRequest($"Group '{q.group_name}' not found");
                    }

                    groups.Add(new FeedbackGroup
                    {
                        FeedbackId = feedback.FeedbackId,
                        GroupId = groupEntity.group_id,
                        StaffId = q.staff_id
                    });
                }

                _db.FeedbackGroup.AddRange(groups);
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                Message = "Feedback created successfully",
                FeedbackId = feedback.FeedbackId
            });
        }
        [Route("DeleteFeedback/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var feedback = await _db.Feedback
                .Include(f => f.FeedbackGroups) 
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (feedback == null)
            {
                return NotFound(new { Message = "Feedback not found" });
            }

         
            if (feedback.FeedbackGroups != null && feedback.FeedbackGroups.Any())
            {
                _db.FeedbackGroup.RemoveRange(feedback.FeedbackGroups);
            }

          
            _db.Feedback.Remove(feedback);
            await _db.SaveChangesAsync();

            return Ok(new { Message = "Feedback deleted successfully", FeedbackId = id });
        }


        [Route("UpdateFeedback/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackCreateDto dto)
        {
            if (dto == null || id != dto.FeedbackId)
                return BadRequest("Invalid data");

            var feedback = await _db.Feedback
                .Include(f => f.FeedbackGroups) 
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (feedback == null) return NotFound("Feedback not found");

          
            feedback.course_id = dto.course_id;
            feedback.module_id = dto.module_id;
            feedback.feedback_type_id = dto.feedback_type_id;
            feedback.session = dto.session;
            feedback.start_date = dto.start_date;
            feedback.end_date = dto.end_date;
            feedback.status = dto.status;

           
            _db.FeedbackGroup.RemoveRange(feedback.FeedbackGroups);

            if (dto.Groups != null && dto.Groups.Any())
            {
                var groups = new List<FeedbackGroup>();

                foreach (var q in dto.Groups)
                {
                    var groupEntity = await _db.Groups
                        .FirstOrDefaultAsync(g => g.group_name == q.group_name);

                    if (groupEntity == null)
                    {
                        return BadRequest($"Group '{q.group_name}' not found");
                    }

                    groups.Add(new FeedbackGroup
                    {
                        FeedbackId = feedback.FeedbackId,
                        GroupId = groupEntity.group_id,
                        StaffId = q.staff_id
                    });
                }

                _db.FeedbackGroup.AddRange(groups);
            }

            await _db.SaveChangesAsync();

            return Ok(new { Message = "Feedback updated successfully" });
        }


    }


}











    
