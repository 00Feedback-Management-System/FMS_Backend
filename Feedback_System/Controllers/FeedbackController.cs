using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Feedback_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public FeedbackController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost("Schedule")]
        public async Task<IActionResult> ScheduleFeedback([FromBody] FeedbackDto dto)
        {
            if (dto == null) return BadRequest("Invalid data");

            var feedback = new Feedback
            {
                Course = await _context.Courses.FindAsync(dto.CourseId),
                Module = await _context.Modules.FindAsync(dto.ModuleId),
                FeedbackType = await _context.FeedbackType.FindAsync(dto.FeedbackTypeId),
                session = dto.Session,
                start_date = dto.StartDate,
                end_date = dto.EndDate,
                status = "active"
            };

            _context.Feedback.Add(feedback);
            await _context.SaveChangesAsync();

            // If multiple groups
            if (dto.FeedbackGroups != null && dto.FeedbackGroups.Any())
            {
                foreach (var fg in dto.FeedbackGroups)
                {
                    _context.FeedbackGroup.Add(new FeedbackGroup
                    {
                        FeedbackId = feedback.FeedbackId,
                        GroupId = fg.GroupId,
                        StaffId = fg.StaffId
                    });
                }
                await _context.SaveChangesAsync();
            }
            else if (dto.StaffId.HasValue) // Single staff
            {
                _context.FeedbackGroup.Add(new FeedbackGroup
                {
                    FeedbackId = feedback.FeedbackId,
                    StaffId = dto.StaffId
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new { Message = "Feedback scheduled successfully!" });
        }
        [HttpPut("Update/{feedbackId}")]
        public async Task<IActionResult> UpdateFeedback(int feedbackId, [FromBody] FeedbackDto dto)
        {
            if (dto == null) return BadRequest("Invalid data");

            var feedback = await _context.Feedback
                .Include(f => f.FeedbackGroups)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);

            if (feedback == null)
                return NotFound(new { Message = "Feedback not found!" });

            
       

            // ✅ Update feedback fields
            feedback.course_id = dto.CourseId ?? feedback.course_id;
            feedback.module_id = dto.ModuleId ?? feedback.module_id;
            feedback.feedback_type_id = dto.FeedbackTypeId ?? feedback.feedback_type_id;
            feedback.session = dto.Session;
            feedback.start_date = dto.StartDate;
            feedback.end_date = dto.EndDate;
            feedback.status = dto.Status ?? feedback.status;

            // ✅ Handle FeedbackGroups individually
            if (dto.FeedbackGroups != null && dto.FeedbackGroups.Any())
            {
                var existingGroups = feedback.FeedbackGroups.ToList();

                // Update or Insert
                foreach (var fgDto in dto.FeedbackGroups)
                {
                    if (fgDto.FeedbackGroupId > 0)
                    {
                        // Update existing group
                        var existing = existingGroups.FirstOrDefault(g => g.FeedbackGroupId == fgDto.FeedbackGroupId);
                        if (existing != null)
                        {
                            existing.GroupId = fgDto.GroupId;
                            existing.StaffId = fgDto.StaffId;
                        }
                    }
                    else
                    {
                        // Insert new group
                        _context.FeedbackGroup.Add(new FeedbackGroup
                        {
                            FeedbackId = feedback.FeedbackId,
                            GroupId = fgDto.GroupId,
                            StaffId = fgDto.StaffId
                        });
                    }
                }

                // Delete groups not in DTO
                var dtoIds = dto.FeedbackGroups.Select(g => g.FeedbackGroupId).ToList();
                var toRemove = existingGroups.Where(g => !dtoIds.Contains(g.FeedbackGroupId)).ToList();
                if (toRemove.Any())
                    _context.FeedbackGroup.RemoveRange(toRemove);
            }
            else if (dto.StaffId.HasValue) // Single staff case
            {
                // Ensure at least one record exists or create one
                var singleGroup = feedback.FeedbackGroups.FirstOrDefault();
                if (singleGroup != null)
                {
                    singleGroup.StaffId = dto.StaffId;
                    singleGroup.GroupId = null;
                }
                else
                {
                    _context.FeedbackGroup.Add(new FeedbackGroup
                    {
                        FeedbackId = feedback.FeedbackId,
                        StaffId = dto.StaffId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Feedback updated successfully!" });
        }



        [HttpGet("GetFeedback")]
        public async Task<IActionResult> GetScheduledFeedback()
        {
            var feedbacks = await _context.Feedback
                .Include(f => f.Course)
                .Include(f => f.Module)
                .Include(f => f.FeedbackType)
                .Include(f => f.FeedbackGroups)
                    .ThenInclude(fg => fg.Staff)
                .Include(f => f.FeedbackGroups)
                    .ThenInclude(fg => fg.Groups)
                .ToListAsync();

            var result = feedbacks
                .SelectMany(f => f.FeedbackGroups.Select(fg => new
                {
                    FeedbackGroupId = fg.FeedbackGroupId, // ✅ unique per row
                    FeedbackId = f.FeedbackId,
                    CourseName = f.Course.course_name,
                    ModuleName = f.Module.module_name,
                    FeedbackTypeName = f.FeedbackType.feedback_type_title,
                    FeedbackTypeId = f.feedback_type_id,
                    StaffName = fg.Staff != null
                                ? fg.Staff.first_name + " " + fg.Staff.last_name
                                : "-",
                    GroupName = fg.Groups != null
                                ? fg.Groups.group_name
                                : "-",
                    Session = f.session,
                    StartDate = f.start_date,
                    EndDate = f.end_date,
                    Status = f.status
                }))
                .ToList();

            return Ok(result);
        }

        // 🔹 GET by FeedbackId
        [HttpGet("GetByFeedback/{feedbackId}")]
        public async Task<IActionResult> GetByFeedback(int feedbackId)
        {
            var feedback = await _context.Feedback
                .Include(f => f.FeedbackGroups)
                    .ThenInclude(fg => fg.Staff)
                .Include(f => f.FeedbackGroups)
                    .ThenInclude(fg => fg.Groups)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);

            if (feedback == null)
                return NotFound(new { message = "Feedback not found" });

            var dto = new FeedbackWithGroupsDto
            {
                FeedbackId = feedback.FeedbackId,
                CourseId = feedback.course_id,
                ModuleId = feedback.module_id,
                FeedbackTypeId = feedback.feedback_type_id,
                Session = feedback.session,
                StartDate = feedback.start_date,
                EndDate = feedback.end_date,
                Status = feedback.status,
                FeedbackGroups = feedback.FeedbackGroups.Select(fg => new FeedbackGroupDto
                {
                    FeedbackGroupId = fg.FeedbackGroupId,
                    // 🔹 If it's a single-group feedback, GroupId should stay null instead of 0
                    GroupId = fg.GroupId,
                    StaffId = fg.StaffId ?? 0
                }).ToList()
            };

            return Ok(dto);
        }



        [HttpDelete("DeleteFeedbackGroup/{feedbackGroupId}")]
        public async Task<IActionResult> DeleteFeedbackGroup(int feedbackGroupId)
        {
            try
            {
                var feedbackGroup = await _context.FeedbackGroup
                    .Include(fg => fg.Feedback) // include feedback so we can check start_date
                    .FirstOrDefaultAsync(fg => fg.FeedbackGroupId == feedbackGroupId);

                if (feedbackGroup == null)
                {
                    return NotFound(new { message = "FeedbackGroup not found" });
                }

                var feedback = feedbackGroup.Feedback;
                if (feedback == null)
                {
                    return BadRequest(new { message = "Feedback reference not found" });
                }

                // ⏳ Check if current date >= start_date
                if (DateTime.Now >= feedback.start_date)
                {
                    return BadRequest(new { message = "Cannot delete feedback after start date" });
                }

                int feedbackId = feedbackGroup.FeedbackId ?? 0;

                // ✅ Remove the FeedbackGroup row
                _context.FeedbackGroup.Remove(feedbackGroup);
                await _context.SaveChangesAsync();

                // ✅ Check if there are any remaining groups for this FeedbackId
                bool hasRemainingGroups = await _context.FeedbackGroup
                    .AnyAsync(fg => fg.FeedbackId == feedbackId);

                // ✅ If no groups left → delete the main Feedback record also
                if (!hasRemainingGroups)
                {
                    _context.Feedback.Remove(feedback);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Feedback group deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting feedback group", error = ex.Message });
            }
        }

        [HttpGet("GetSubmittedFeedbackIdsByStudentId/{studentId}")]
        public async Task<ActionResult<List<int>>> GetSubmittedFeedbackIdsByStudentId(int studentId)
        {
            var submittedFeedbackIds = await _context.FeedbackSubmits
                                                     .Where(fs => fs.student_rollno == studentId)
                                                     .Select(fs => fs.feedback_id)
                                                     .ToListAsync();

            return Ok(submittedFeedbackIds);
        }

        [HttpGet("GetSubmittedFeedbackHistory/{studentId}")]
        public async Task<ActionResult<IEnumerable<SubmittedFeedbackHistoryDto>>> GetSubmittedFeedbackHistory(int studentId)
        {
            var submittedFeedbacks = await (from fs in _context.FeedbackSubmits
                                            where fs.student_rollno == studentId
                                            join fg in _context.FeedbackGroup on fs.feedback_id equals fg.FeedbackGroupId
                                            join f in _context.Feedback on fg.FeedbackId equals f.FeedbackId
                                            join ft in _context.FeedbackType on f.feedback_type_id equals ft.feedback_type_id
                                            join c in _context.Courses on f.course_id equals c.course_id
                                            join m in _context.Modules on f.module_id equals m.module_id
                                            join s in _context.Staff on fg.StaffId equals s.staff_id
                                            select new SubmittedFeedbackHistoryDto
                                            {
                                                FeedbackGroupId = fg.FeedbackGroupId,
                                                FeedbackId = f.FeedbackId,
                                                FeedbackTypeId = ft.feedback_type_id,
                                                FeedbackTypeName = ft.feedback_type_title,
                                                CourseName = c.course_name,
                                                ModuleName = m.module_name,
                                                StaffName = s.first_name + " " + s.last_name,
                                                Session = f.session,
                                                SubmittedAt = fs.submited_at
                                            }).ToListAsync();
            if(submittedFeedbacks == null || !submittedFeedbacks.Any())
            {
                return Ok(new List<SubmittedFeedbackHistoryDto>());
            }

            return Ok(submittedFeedbacks);
        }

        [HttpGet("GetSubmittedFeedbackDetailsForView/{feedbackGroupId}/{studentRollNo}")]
        public async Task<ActionResult<SubmittedFeedbackDetailsDto>> GetSubmittedFeedbackDetailsForStudentAndForm(int feedbackGroupId, int studentRollNo)
        {
            var feedbackDetails = await (from fs in _context.FeedbackSubmits
                                         where fs.feedback_id == feedbackGroupId && fs.student_rollno == studentRollNo
                                         join fg in _context.FeedbackGroup on fs.feedback_id equals fg.FeedbackGroupId
                                         join f in _context.Feedback on fg.FeedbackId equals f.FeedbackId
                                         join ft in _context.FeedbackType on f.feedback_type_id equals ft.feedback_type_id
                                         join c in _context.Courses on f.course_id equals c.course_id
                                         join m in _context.Modules on f.module_id equals m.module_id
                                         join s in _context.Staff on fg.StaffId equals s.staff_id
                                         select new SubmittedFeedbackDetailsDto
                                         {
                                             FeedbackGroupId = fg.FeedbackGroupId,
                                             FeedbackTypeName = ft.feedback_type_title,
                                             CourseName = c.course_name,
                                             ModuleName = m.module_name,
                                             StaffName = s.first_name + " " + s.last_name,
                                             Session = f.session,
                                             Answers = new List<FeedbackAnswerDto>() 
                                         }).FirstOrDefaultAsync();

            if (feedbackDetails == null)
            {
                return NotFound("Feedback details not found.");
            }

            var answers = await (from a in _context.FeedbackAnswers
                                 join fs in _context.FeedbackSubmits on a.feedback_submit_id equals fs.feedback_submit_id
                                 where fs.feedback_id == feedbackGroupId && fs.student_rollno == studentRollNo
                                 join q in _context.FeedbackQuestions on a.question_id equals q.question_id
                                 select new FeedbackAnswerDto
                                 {
                                     AnswerId = a.answer_id,
                                     QuestionId = q.question_id,
                                     QuestionText = q.question,
                                     AnswerText = a.answer
                                 }).ToListAsync();

            feedbackDetails.Answers = answers;

            return Ok(feedbackDetails);
        }
    }

}
