using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Feedback_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Feedback_System.Controllers
{
    [Route("api/staff")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        public readonly ApplicationDBContext _db;
        private readonly PasswordServices _passwordServices;

        public StaffController(ApplicationDBContext dbContext, PasswordServices passwordServices)
        {
            _db = dbContext;
            _passwordServices = passwordServices;
        }

        [HttpPost("addStaff")]
        public IActionResult AddStaff([FromBody] StaffDTO staffDTO)
        {
            try
            {
                if (staffDTO == null)
                    return BadRequest(new { message = "Invalid request data" });

                var existingStaff = _db.Staff.FirstOrDefault(e => e.email == staffDTO.email);
                if (existingStaff != null)
                    return BadRequest(new { message = "Email already registered" });

                var hashedPassword = _passwordServices.HashPassword(staffDTO.password);

                Staff staff = new Staff
                {
                    staffrole_id = staffDTO.staffrole_id,
                    first_name = staffDTO.first_name,
                    last_name = staffDTO.last_name,
                    email = staffDTO.email,
                    password = hashedPassword,
                    profile_image = staffDTO.profile_image
                };

                _db.Staff.Add(staff);
                _db.SaveChanges();

                return Ok(new { message = "Staff added succesfully", staffid = staff.staff_id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("getAllStaff")]
        public IActionResult GetAllStaff()
        {
            try
            {
                var staffList = _db.Staff.Select(s => new
                {
                    s.staff_id,
                    s.staffrole_id,
                    s.first_name,
                    s.last_name,
                    s.email,
                    s.profile_image
                }).ToList();

                if (!staffList.Any())
                    return NotFound(new { message = "No Staff records found" });

                return Ok(staffList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("getStaff/{id}")]
        public IActionResult GetStaffById(int id)
        {
            try
            {
                var staff = _db.Staff.FirstOrDefault(s => s.staff_id == id);
                if (staff == null)
                    return NotFound(new { message = "Staff not found" });

                return Ok(new
                {
                    staff.staff_id,
                    staff.staffrole_id,
                    staff.first_name,
                    staff.last_name,
                    staff.email,
                    staff.profile_image,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("updateStaff/{id}")]
        public IActionResult UpdateStaff(int id, [FromBody] StaffDTO staffDTO)
        {
            try
            {
                var staff = _db.Staff.FirstOrDefault(s => s.staff_id == id);
                if (staff == null)
                    return NotFound(new { message = "Staff not found" });

                staff.staffrole_id = staffDTO.staffrole_id;
                staff.first_name = staffDTO.first_name;
                staff.last_name = staffDTO.last_name;
                staff.email = staffDTO.email;
                if(!string.IsNullOrEmpty(staffDTO.password))
                {
                    staff.password = _passwordServices.HashPassword(staffDTO.password);
                }
                staff.profile_image = staffDTO.profile_image;

                _db.Staff.Update(staff);
                _db.SaveChanges();

                return Ok(new { message= "Staff updated successfully"});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deleteStaff/{id}")]
        public IActionResult DeleteStaff(int id)
        {
            try
            {
                var staff = _db.Staff.FirstOrDefault(s => s.staff_id == id);
                if (staff == null)
                    return NotFound(new { message = "Staff not found" });

                _db.Staff.Remove(staff);
                _db.SaveChanges();

                return Ok(new { message = "Staff deleted successfully" });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("{staffId}/scheduledFeedback")]
        public async Task<IActionResult> GetScheduledFeedbackForStaff(int staffId)
        {
           
            var feedbacks = await _db.Feedback
                .Include(f => f.Course)
                .Include(f => f.Module)
                .Include(f => f.FeedbackType)
                .Include(f => f.FeedbackGroups)
                    .ThenInclude(fg => fg.Staff)
                .Include(f => f.FeedbackGroups)
                    .ThenInclude(fg => fg.Groups)
                .Where(f => f.FeedbackGroups.Any(fg => fg.StaffId == staffId))
                .ToListAsync();

            if (!feedbacks.Any())
                return NotFound(new { message = "No scheduled feedback found for this staff." });

           
            var staffRatings = await (from fa in _db.FeedbackAnswers
                                      join fq in _db.FeedbackQuestions on fa.question_id equals fq.question_id
                                      join fs in _db.FeedbackSubmits on fa.feedback_submit_id equals fs.feedback_submit_id
                                      join fg in _db.FeedbackGroup on fs.feedback_group_id equals fg.FeedbackGroupId
                                      join f in _db.Feedback on fg.FeedbackId equals f.FeedbackId
                                      join ft in _db.FeedbackType on f.feedback_type_id equals ft.feedback_type_id
                                      where fg.StaffId == staffId &&
                                            (fq.question_type == "mcq" || fq.question_type == "rating")
                                      select new
                                      {
                                          f.FeedbackId,
                                          fs.feedback_submit_id,
                                          ft.feedback_type_id,
                                          AnswerValue = fa.answer,
                                          QuestionType = fq.question_type
                                      })
                                      .ToListAsync();

           
            var mappedRatings = staffRatings.Select(x => new
            {
                x.FeedbackId,
                x.feedback_submit_id,
                x.feedback_type_id,
                Value = x.QuestionType == "mcq"
                    ? MapMcqAnswerToNumber(x.AnswerValue)
                    : int.TryParse(x.AnswerValue, out var val) ? val : 0
            }).ToList();

            
            var feedbackRatings = mappedRatings
                .GroupBy(x => new { x.FeedbackId, x.feedback_submit_id, x.feedback_type_id })
                .ToDictionary(
                    g => new { g.Key.FeedbackId, g.Key.feedback_submit_id, g.Key.feedback_type_id },
                    g => Math.Round(g.Average(x => x.Value), 2)
                );

            
            var result = feedbacks
                .SelectMany(f => f.FeedbackGroups
                    .Where(fg => fg.StaffId == staffId)
                    .SelectMany(fg => _db.FeedbackSubmits
                        .Where(fs => fs.feedback_group_id == fg.FeedbackGroupId)
                        .AsEnumerable() 
                        .Select(fs => new
                        {
                            FeedbackId = f.FeedbackId,
                            feedback_type_id = f.FeedbackType.feedback_type_id,
                            CourseName = f.Course.course_name,
                            ModuleName = f.Module.module_name,
                            FeedbackTypeName = f.FeedbackType.feedback_type_title,
                            StaffName = fg.Staff != null
                                        ? fg.Staff.first_name + " " + fg.Staff.last_name
                                        : "-",
                            GroupName = fg.Groups != null
                                        ? fg.Groups.group_name
                                        : "-",
                            Session = f.session,
                            StartDate = f.start_date,
                            EndDate = f.end_date,
                            Status = f.status,
                            Rating = feedbackRatings.TryGetValue(
                                new { f.FeedbackId, fs.feedback_submit_id, f.FeedbackType.feedback_type_id },
                                out var avg) ? avg : 0
                        })))
                .ToList();

            return Ok(result);
        }

        
        private int MapMcqAnswerToNumber(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return 0;

            return answer.Trim().ToLower() switch
            {
                "excellent" => 5,
                "good" => 4,
                "average" => 3,
                "poor" => 2,
                "very poor" => 1,
                "yes" => 5,
                "no" => 1,
                _ => int.TryParse(answer, out var val) ? val : 0
            };
        }

        
    }
}

