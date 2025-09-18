using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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
                                                     .Select(fs => fs.feedback_group_id)
                                                     .ToListAsync();

            return Ok(submittedFeedbackIds);
        }

        [HttpGet("GetSubmittedFeedbackHistory/{studentId}")]
        public async Task<ActionResult> GetSubmittedFeedbackHistory(int studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.student_rollno == studentId);
            if (student == null)
            {
                return NotFound(new { message = "Student not found." });
            }

            var submittedFeedbacks = await (from fs in _context.FeedbackSubmits
                                            join fg in _context.FeedbackGroup on fs.feedback_group_id equals fg.FeedbackGroupId
                                            join f in _context.Feedback on fg.FeedbackId equals f.FeedbackId
                                            join ft in _context.FeedbackType on f.feedback_type_id equals ft.feedback_type_id
                                            join c in _context.Courses on f.course_id equals c.course_id
                                            join m in _context.Modules on f.module_id equals m.module_id
                                            join s in _context.Staff on fg.StaffId equals s.staff_id
                                            where fs.student_rollno == studentId || fg.GroupId == student.group_id
                                            select new
                                            {
                                                feedbackGroupId = fg.FeedbackGroupId,
                                                feedbackId = f.FeedbackId,
                                                feedbackTypeId = ft.feedback_type_id,
                                                feedbackTypeName = ft.feedback_type_title,
                                                courseName = c.course_name,
                                                moduleName = m.module_name,
                                                staffName = s.first_name + " " + s.last_name,
                                                session = f.session,
                                                submittedAt = fs.submited_at,
                                                groupName = fg.GroupId
                                            }).ToListAsync();

            return Ok(submittedFeedbacks);
        }

        [HttpGet("GetSubmittedFeedbackDetailsForView/{feedbackGroupId}/{studentRollNo}")]
        public async Task<ActionResult<SubmittedFeedbackDetailsDto>> GetSubmittedFeedbackDetailsForStudentAndForm(int feedbackGroupId, int studentRollNo)
        {
            var feedbackDetails = await (from fs in _context.FeedbackSubmits
                                         where fs.feedback_group_id == feedbackGroupId && fs.student_rollno == studentRollNo
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
                                 where fs.feedback_group_id == feedbackGroupId && fs.student_rollno == studentRollNo
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

        [HttpGet("GetScheduledFeedbackByStudent/{studentRollNo}")]
        public async Task<IActionResult> GetScheduledFeedbackByStudent(int studentRollNo, [FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                var student = await _context.Students
                                        .Include(s => s.Groups)
                                        .FirstOrDefaultAsync(s => s.student_rollno == studentRollNo);

                if (student == null)
                {
                    return NotFound(new { message = "Student not found" });
                }

                var studentGroupId = student.group_id;
                //if (studentGroupId == null)
                //{
                //    return BadRequest(new { message = "Student is not assigned to any group" });
                //}

                var courseGroup = await _context.CourseGroups
                                    .Include(cg => cg.Course)
                                    .FirstOrDefaultAsync(cg => cg.group_id == studentGroupId);

                if (courseGroup == null || courseGroup.Course == null)
                {
                    return Ok(new List<object>());
                }

                var studentCourseId = courseGroup.course_id;

                var submittedFeedbackIds = await _context.FeedbackSubmits
                                                        .Where(fs => fs.student_rollno == studentRollNo)
                                                        .Select(fs => fs.feedback_group_id)
                                                        .Distinct()
                                                        .ToListAsync();

                var pendingFeedbacksQuery = _context.Feedback
                                            .Where(f => f.course_id == studentCourseId && f.status == "active")
                                            .SelectMany(f => f.FeedbackGroups
                                                .Where(fg => (fg.GroupId == studentGroupId || fg.GroupId == null) &&
                                                             !submittedFeedbackIds.Contains(fg.FeedbackGroupId))
                                                .Select(fg => new
                                                {
                                                    FeedbackGroupId = fg.FeedbackGroupId,
                                                    FeedbackId = f.FeedbackId,
                                                    CourseName = f.Course.course_name,
                                                    ModuleName = f.Module.module_name,
                                                    FeedbackTypeName = f.FeedbackType.feedback_type_title,
                                                    FeedbackTypeId = f.feedback_type_id,
                                                    StaffName = fg.Staff != null ? fg.Staff.first_name + " " + fg.Staff.last_name : "-",
                                                    GroupName = fg.Groups != null ? fg.Groups.group_name : "-",
                                                    Session = f.session,
                                                    StartDate = f.start_date,
                                                    EndDate = f.end_date,
                                                    Status = f.status
                                                }));

                var totalCount = await pendingFeedbacksQuery.CountAsync();

                var result = await pendingFeedbacksQuery
                                    .OrderByDescending(f => f.FeedbackGroupId)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

                var paginatedResult = new
                {
                    Data = result,
                    TotalCount = totalCount
                };

                return Ok(paginatedResult);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving scheduled feedback", error = ex.Message });
            }
        }

      
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        //get data for admin feedback dashboard and rating
        [HttpGet("FeedbackDashboard-Rating")]
        public async Task<IActionResult> GetFeedbackDashboard()
        {
            var feedbackSubmits = await _context.FeedbackSubmits
                .Include(fs => fs.Feedback)
                    .ThenInclude(f => f.Course)
                .Include(fs => fs.Feedback)
                    .ThenInclude(f => f.Module)
                .Include(fs => fs.Feedback)
                    .ThenInclude(f => f.FeedbackType)
                .Include(fs => fs.FeedbackGroup)
                    .ThenInclude(fg => fg.Staff)
                .Include(fs => fs.FeedbackGroup)
                    .ThenInclude(fg => fg.Groups)
                .ToListAsync();

            // Get all answers with question type for submitted feedbacks
            var feedbackSubmitIds = feedbackSubmits.Select(fs => fs.feedback_submit_id).ToList();

            var answers = await (from fa in _context.FeedbackAnswers
                                 join fq in _context.FeedbackQuestions on fa.question_id equals fq.question_id
                                 where feedbackSubmitIds.Contains(fa.feedback_submit_id ?? 0)
                                       && (fq.question_type == "mcq" || fq.question_type == "rating")
                                 select new
                                 {
                                     FeedbackSubmitId = fa.feedback_submit_id,
                                     QuestionType = fq.question_type,
                                     AnswerValue = fa.answer
                                 }).ToListAsync();

            // Helper for MCQ mapping
            int MapMcqAnswerToNumber(string answer) => answer switch
            {
                "Excellent" => 5,
                "Good" => 4,
                "Average" => 3,
                "Poor" => 2,
                "Very Poor" => 1,
                _ => 0
            };

            var result = feedbackSubmits
    .Select(fs =>
    {
        var submitAnswers = answers.Where(a => a.FeedbackSubmitId == fs.feedback_submit_id);

        var ratingValues = submitAnswers.Select(a =>
            a.QuestionType == "mcq"
                ? MapMcqAnswerToNumber(a.AnswerValue)
                : int.TryParse(a.AnswerValue, out var val) ? val : 0
        ).ToList();

        double staffRating = ratingValues.Any() ? ratingValues.Average() : 0;

        return new
        {
            FeedbackGroupId = fs.FeedbackGroup?.FeedbackGroupId,
            FeedbackId = fs.Feedback?.FeedbackId,
            CourseName = fs.Feedback?.Course?.course_name ?? "-",
            ModuleName = fs.Feedback?.Module?.module_name ?? "-",
            FeedbackTypeName = fs.Feedback?.FeedbackType?.feedback_type_title ?? "-",
            FeedbackTypeId = fs.Feedback?.feedback_type_id,
            StaffName = fs.FeedbackGroup?.Staff != null
                        ? fs.FeedbackGroup.Staff.first_name + " " + fs.FeedbackGroup.Staff.last_name
                        : "-",
            GroupName = fs.FeedbackGroup?.Groups?.group_name ?? "-",
            Session = fs.Feedback?.session,
            StartDate = fs.Feedback?.start_date,
            EndDate = fs.Feedback?.end_date,
            Status = fs.Feedback?.status,
            SubmittedAt = fs.submited_at,
            StudentRollNo = fs.student_rollno,
            StaffRating = staffRating
        };
    })
    .ToList();

            // ✅ Group by FeedbackId and take average
            var groupedResult = result
                .GroupBy(r => new
                {
                    r.FeedbackId,
                    r.FeedbackGroupId,
                    r.CourseName,
                    r.ModuleName,
                    r.FeedbackTypeName,
                    r.FeedbackTypeId,
                    r.StaffName,
                    r.GroupName,
                    r.Session,
                    r.StartDate,
                    r.EndDate,
                    r.Status
                })
                .Select(g => new
                {
                    g.Key.FeedbackGroupId,
                    g.Key.FeedbackId,
                    g.Key.CourseName,
                    g.Key.ModuleName,
                    g.Key.FeedbackTypeName,
                    g.Key.FeedbackTypeId,
                    g.Key.StaffName,
                    g.Key.GroupName,
                    g.Key.Session,
                    g.Key.StartDate,
                    g.Key.EndDate,
                    g.Key.Status,
                    AverageStaffRating = g.Average(x => x.StaffRating),
                    SubmittedCount = g.Count()
                })
                .ToList();

            return Ok(groupedResult);


            return Ok(result);
        }

        //staff rating 
        [HttpGet("StaffRating")]
        public async Task<IActionResult> Rating()
        {
            // Get all answers with related staff and question type
            var staffRatings = await (from fa in _context.FeedbackAnswers
                                      join fq in _context.FeedbackQuestions on fa.question_id equals fq.question_id
                                      join fs in _context.FeedbackSubmits on fa.feedback_submit_id equals fs.feedback_submit_id
                                      join fg in _context.FeedbackGroup on fs.feedback_group_id equals fg.FeedbackGroupId
                                      join s in _context.Staff on fg.StaffId equals s.staff_id
                                      where fq.question_type == "mcq" || fq.question_type == "rating"
                                      select new
                                      {
                                          StaffId = s.staff_id,
                                          StaffName = s.first_name + " " + s.last_name,
                                          QuestionType = fq.question_type,
                                          AnswerValue = fa.answer // For MCQ, map text to number if needed
                                      })
                                      .ToListAsync();

            // If MCQ answers are text, map them to numbers here
            var mappedRatings = staffRatings.Select(x => new
            {
                x.StaffId,
                x.StaffName,
                Value = x.QuestionType == "mcq"
                    ? MapMcqAnswerToNumber(x.AnswerValue) // Implement this mapping function
                    : int.TryParse(x.AnswerValue, out var val) ? val : 0
            });

            var result = mappedRatings
                .GroupBy(x => new { x.StaffId, x.StaffName })
                .Select(g => new
                {
                    StaffId = g.Key.StaffId,
                    StaffName = g.Key.StaffName,
                    AverageRating = g.Select(x => x.Value).DefaultIfEmpty(0).Average()
                })
                .ToList();

            return Ok(result);
        }

        // Example mapping function for MCQ answers
        private int MapMcqAnswerToNumber(string answer)
        {
            return answer switch
            {
                "Excellent" => 5,
                "Good" => 4,
                "Average" => 3,
                "Poor" => 2,
                "Very Poor" => 1,
                _ => 0
            };
        }
        //faculty feedback summary
        [HttpPost("FacultyFeedbackSummary")]
        public async Task<IActionResult> FacultyFeedbackSummary([FromBody] FacultyFeedbackSummaryDto request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request" });

            // 1. Get the feedback type
            var feedbackType = await _context.FeedbackType
                .FirstOrDefaultAsync(ft => ft.feedback_type_title == request.type_name);

            if (feedbackType == null)
                return NotFound(new { message = "Feedback type not found." });

            // 2. Get matching feedback groups
            var feedbackGroups = await (
                from fg in _context.FeedbackGroup
                join f in _context.Feedback on fg.FeedbackId equals f.FeedbackId
                join c in _context.Courses on f.course_id equals c.course_id
                join m in _context.Modules on f.module_id equals m.module_id
                join s in _context.Staff on fg.StaffId equals s.staff_id
                where c.course_name == request.course_name
                      && m.module_name == request.module_name
                        && (s.first_name + " " + s.last_name) == request.staff_name   // ✅ staff full name

                select fg
            ).ToListAsync();


            if (!feedbackGroups.Any())
                return NotFound(new { message = "No feedback groups found." });

            var feedbackGroupIds = feedbackGroups.Select(fg => fg.FeedbackGroupId).ToList();
            // 3. Submitted & Remaining count
            // var submittedCount = await _context.FeedbackSubmits
            //   .CountAsync(fs => feedbackGroupIds.Contains(fs.feedback_group_id ?? 0));

            // ✅ Use Students table directly
            //var groupStudentCount = await (from st in _context.Students
            //                               join g in _context.Groups on st.group_id equals g.group_id
            //                               join fg in _context.FeedbackGroup on g.group_id equals fg.GroupId
            //                               where feedbackGroupIds.Contains(fg.FeedbackGroupId)
            //                               select st).CountAsync();

            //var remainingCount = groupStudentCount - submittedCount;

            // pick first feedbackGroup to resolve course & group
            var firstFeedbackGroup = await _context.FeedbackGroup
                .Include(fg => fg.Feedback)
                .FirstOrDefaultAsync(fg => feedbackGroupIds.Contains(fg.FeedbackGroupId));

            int totalStudents = 0;
            if (firstFeedbackGroup != null)
            {
                int courseId = firstFeedbackGroup.Feedback.course_id;

                // get students enrolled in course
                var courseStudents = from cs in _context.CourseStudents
                                     join st in _context.Students on cs.student_rollno equals st.student_rollno
                                     where cs.course_id == courseId
                                     select st;

                // filter by group if group assigned
                if (firstFeedbackGroup.GroupId.HasValue)
                {
                    int groupId = firstFeedbackGroup.GroupId.Value;
                    courseStudents = courseStudents.Where(st => st.group_id == groupId);
                }

                totalStudents = await courseStudents.CountAsync();
            }

            var submittedCount = await _context.FeedbackSubmits
                .CountAsync(fs => feedbackGroupIds.Contains(fs.feedback_group_id ?? 0));

            var remainingCount = totalStudents - submittedCount;


            // 4. Fetch all answers with question info
            var answers = await (from fa in _context.FeedbackAnswers
                                 join fq in _context.FeedbackQuestions on fa.question_id equals fq.question_id
                                 join fs in _context.FeedbackSubmits on fa.feedback_submit_id equals fs.feedback_submit_id
                                 where feedbackGroupIds.Contains(fs.feedback_group_id ?? 0)
                                       && fq.feedback_type_id == feedbackType.feedback_type_id
                                 select new
                                 {
                                     fq.question_id,
                                     fq.question,
                                     fq.question_type,
                                     fa.answer
                                 }).ToListAsync();

            // 5. Helper: Map MCQ → number
            int MapMcqAnswerToNumber(string ans) => ans switch
            {
                "Excellent" => 5,
                "Good" => 4,
                "Average" => 3,
                "Poor" => 2,
                "Very Poor" => 1,
                _ => 0
            };

            // 6. Build question stats
            var questionStats = answers
                .GroupBy(a => new { a.question_id, a.question, a.question_type })
                .Select(g => new
                {
                    QuestionId = g.Key.question_id,
                    QuestionText = g.Key.question,
                    QuestionType = g.Key.question_type,

                    Excellent = g.Count(a => a.answer == "Excellent" || a.answer == "5"),
                    Good = g.Count(a => a.answer == "Good" || a.answer == "4"),
                    Average = g.Count(a => a.answer == "Average" || a.answer == "3"),
                    Poor = g.Count(a => a.answer == "Poor" || a.answer == "2"),
                    VeryPoor = g.Count(a => a.answer == "Very Poor" || a.answer == "1"),

                    Ratings = g.Select(a =>
                        g.Key.question_type == "mcq"
                            ? MapMcqAnswerToNumber(a.answer)
                            : int.TryParse(a.answer, out var val) ? val : 0
                    ).ToList()
                })
                .ToList();

            // 7. Calculate overall average rating
            double avgRating = questionStats
                .SelectMany(q => q.Ratings)
                .DefaultIfEmpty(0)
                .Average();

            // 8. Return response
            return Ok(new
            {
                staff_name = request.staff_name,
                module_name = request.module_name,
                course_name = request.course_name,
                type_name = request.type_name,
                date = request.date,
                Submitted = submittedCount,
                Remaining = remainingCount,
                Rating = avgRating,
                Questions = questionStats.Select(q => new
                {
                    q.QuestionId,
                    q.QuestionText,
                    q.QuestionType,
                    q.Excellent,
                    q.Good,
                    q.Average,
                    q.Poor,
                    q.VeryPoor
                })
            });
        }
    }

}
