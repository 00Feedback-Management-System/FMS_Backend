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



        [HttpGet("GetFeedback")]
        public async Task<IActionResult> GetFeedback()
        {
            var feedbacks = await _db.Feedback
                .Include(f => f.Course)
                .Include(f => f.Module)
                .Include(f => f.FeedbackType)
                .Include(f => f.FeedbackGroups)
                    .ThenInclude(fg => fg.Staff)
                .ToListAsync();

            var result = feedbacks.Select(f => new FeedbackCreateDto
            {
                FeedbackId = f.FeedbackId,
                course_id = f.Course.course_id,
                course_name = f.Course.course_name,
                module_id = f.Module.module_id,
                module_name = f.Module.module_name,
                feedback_type_id = f.FeedbackType.feedback_type_id,
                feedback_type_title = f.FeedbackType.feedback_type_title,
                session = f.session,
                start_date = f.start_date,
                end_date = f.end_date,
                status = f.status,
                staff_id = f.FeedbackGroups.Count == 1 && f.FeedbackGroups.First().GroupId == null
                            ? f.FeedbackGroups.First().StaffId
                            : null,
                Groups = f.FeedbackGroups
                            .Where(fg => fg.GroupId.HasValue)
                            .Select(fg => new FeedbackGroupDto
                            {
                                group_id = fg.GroupId,
                                staff_id = fg.StaffId ?? 0
                            }).ToList()
            }).ToList();

            return Ok(result);
        }



        [HttpPost("CreateFeedback")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data");

            // Create the Feedback entity
            var feedback = new Feedback
            {
                Course = await _db.Courses.FindAsync(dto.course_id),
                Module = await _db.Modules.FindAsync(dto.module_id),
                FeedbackType = await _db.FeedbackType.FindAsync(dto.feedback_type_id),
                session = dto.session,
                start_date = dto.start_date,
                end_date = dto.end_date,
                status = dto.status
            };

            _db.Feedback.Add(feedback);
            await _db.SaveChangesAsync();

            // Multiple groups
            if (dto.Groups != null && dto.Groups.Any())
            {
                foreach (var fg in dto.Groups)
                {
                    _db.FeedbackGroup.Add(new FeedbackGroup
                    {
                        FeedbackId = feedback.FeedbackId,
                        GroupId = fg.group_id, // already int in DTO
                        StaffId = fg.staff_id
                    });
                }

                await _db.SaveChangesAsync();
            }
            // Single staff (no groups)
            else if (dto.staff_id.HasValue)
            {
                _db.FeedbackGroup.Add(new FeedbackGroup
                {
                    FeedbackId = feedback.FeedbackId,
                    StaffId = dto.staff_id.Value
                });

                await _db.SaveChangesAsync();
            }

            return Ok(new { Message = "Feedback scheduled successfully!" });
        }


        [HttpGet("GetFeedback/{id}")]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            var f = await _db.Feedback
                .Include(fb => fb.Course)
                .Include(fb => fb.Module)
                .Include(fb => fb.FeedbackType)
                .Include(fb => fb.FeedbackGroups)
                    .ThenInclude(fg => fg.Staff)
                .Include(fb => fb.FeedbackGroups)
                    .ThenInclude(fg => fg.Groups)
                .Where(fb => fb.FeedbackId == id)
                .Select(fb => new FeedbackCreateDto
                {
                    FeedbackId = fb.FeedbackId,
                    course_id = fb.Course.course_id,
                    course_name = fb.Course.course_name,
                    module_id = fb.Module.module_id,
                    module_name = fb.Module.module_name,
                    feedback_type_id = fb.FeedbackType.feedback_type_id,
                    feedback_type_title = fb.FeedbackType.feedback_type_title,
                    session = fb.session,
                    start_date = fb.start_date,
                    end_date = fb.end_date,
                    status = fb.status,
                    staff_id = fb.FeedbackGroups.Count == 1 && fb.FeedbackGroups.First().GroupId == null
                                ? fb.FeedbackGroups.First().StaffId
                                : null,
                    Groups = fb.FeedbackGroups
                                .Where(fg => fg.GroupId.HasValue)
                                .Select(fg => new FeedbackGroupDto
                                {
                                    group_id = fg.GroupId,
                                    staff_id = fg.StaffId.Value
                                }).ToList()
                })
                .FirstOrDefaultAsync();

            if (f == null) return NotFound("Feedback not found");

            return Ok(f);
        }

        [HttpDelete("DeleteFeedback/{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var feedback = await _db.Feedback
                .Include(f => f.FeedbackGroups)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (feedback == null)
                return NotFound("Feedback not found");

            // Remove associated groups first
            if (feedback.FeedbackGroups != null && feedback.FeedbackGroups.Any())
            {
                _db.FeedbackGroup.RemoveRange(feedback.FeedbackGroups);
            }

            _db.Feedback.Remove(feedback);
            await _db.SaveChangesAsync();

            return Ok(new { Message = "Feedback deleted successfully" });
        }

        [HttpPut("UpdateFeedback/{id}")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data");

            // Find the existing Feedback
            var feedback = await _db.Feedback
                .Include(f => f.FeedbackGroups)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (feedback == null)
                return NotFound("Feedback not found");

            // Update main Feedback fields
            feedback.Course = await _db.Courses.FindAsync(dto.course_id);
            feedback.Module = await _db.Modules.FindAsync(dto.module_id);
            feedback.FeedbackType = await _db.FeedbackType.FindAsync(dto.feedback_type_id);
            feedback.session = dto.session;
            feedback.start_date = dto.start_date;
            feedback.end_date = dto.end_date;
            feedback.status = dto.status;

            // Remove old FeedbackGroups
            if (feedback.FeedbackGroups != null && feedback.FeedbackGroups.Any())
            {
                _db.FeedbackGroup.RemoveRange(feedback.FeedbackGroups);
            }

            // Add new groups or single staff
            if (dto.Groups != null && dto.Groups.Any())
            {
                // Multiple groups
                foreach (var fg in dto.Groups)
                {
                    _db.FeedbackGroup.Add(new FeedbackGroup
                    {
                        FeedbackId = feedback.FeedbackId,
                        GroupId = fg.group_id,
                        StaffId = fg.staff_id
                    });
                }
            }
            else if (dto.staff_id.HasValue)
            {
                // Single staff
                _db.FeedbackGroup.Add(new FeedbackGroup
                {
                    FeedbackId = feedback.FeedbackId,
                    StaffId = dto.staff_id.Value
                });
            }

            await _db.SaveChangesAsync();

            return Ok(new { Message = "Feedback updated successfully!" });
        }





    }


}











    
