using Feedback_System.Data;
using Feedback_System.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Feedback_System.Controllers
{
   

        [ApiController]
        [Route("api/[controller]")]
        public class FeedbackReportController : ControllerBase
        {
            private readonly ApplicationDBContext _context;

            public FeedbackReportController(ApplicationDBContext context)
            {
                _context = context;
            }

        [Authorize(Roles = "Admin")]
        [HttpGet("course-feedback-report")]
        public async Task<IActionResult> GetCourseFeedbackReport()
        {
            // Step 1: only include feedbacks that have at least one submission
            var raw = await (
                from f in _context.Feedback
                join c in _context.Courses on f.course_id equals c.course_id
                join ft in _context.FeedbackType on f.feedback_type_id equals ft.feedback_type_id
                join fs in _context.FeedbackSubmits on f.FeedbackId equals fs.feedback_id
                join fa in _context.FeedbackAnswers on fs.feedback_submit_id equals fa.feedback_submit_id into faj
                from fa in faj.DefaultIfEmpty()
                join fq in _context.FeedbackQuestions on fa.question_id equals fq.question_id into fqj
                from fq in fqj.DefaultIfEmpty()
                select new
                {
                    CourseId = c.course_id,
                    CourseName = c.course_name,
                    FeedbackTypeId = ft.feedback_type_id,
                    FeedbackTypeName = ft.feedback_type_title,
                    GroupType = ft.@group,
                    FeedbackId = f.FeedbackId,
                    FeedbackEndDate = f.end_date,
                    Session = f.session,
                    QuestionType = fq == null ? null : fq.question_type,
                    Answer = fa == null ? null : fa.answer
                }
            ).ToListAsync();

            // Helper: map answer text/type to numeric score
            double MapAnswerToScore(string answer, string questionType)
            {
                if (string.IsNullOrWhiteSpace(answer) || string.IsNullOrWhiteSpace(questionType))
                    return 0.0;

                questionType = questionType.Trim().ToLowerInvariant();
                if (questionType == "rating")
                {
                    if (double.TryParse(answer, System.Globalization.NumberStyles.Any,
                                        System.Globalization.CultureInfo.InvariantCulture, out var d))
                        return d;
                    return 0.0;
                }

                if (questionType == "mcq")
                {
                    switch (answer.Trim().ToLowerInvariant())
                    {
                        case "excellent": return 5.0;
                        case "good": return 4.0;
                        case "average": return 3.0;
                        case "poor": return 2.0;
                        case "very poor": return 1.0;
                        default: return 0.0;
                    }
                }

                return 0.0; // descriptive or unknown types
            }

            // Step 2: group by Course + FeedbackType + GroupType
            var grouped = raw
                .GroupBy(x => new { x.CourseId, x.CourseName, x.FeedbackTypeId, x.FeedbackTypeName, Group = x.GroupType })
                .Select(g =>
                {
                    var sessionsSum = g
                        .GroupBy(x => x.FeedbackId)
                        .Select(gr => gr.First().Session)
                        .Sum();

                    var maxDate = g.Max(x => x.FeedbackEndDate);

                    var scores = g
                        .Select(x => MapAnswerToScore(x.Answer, x.QuestionType))
                        .Where(v => v > 0)
                        .ToList();

                    double avgRating = scores.Count > 0 ? Math.Round(scores.Average(), 2) : 0.0;

                    return new CourseFeedbackReportDto
                    {
                        Date = maxDate,
                        CourseName = g.Key.CourseName,
                        FeedbackTypeName = g.Key.FeedbackTypeName,
                        Groups = g.Key.Group ?? "Unknown",
                        Sessions = sessionsSum,
                        Rating = avgRating
                    };
                })
                .ToList();

            // Sort results
            var sorted = grouped
                .OrderBy(x => x.CourseName)
                .ThenBy(x => x.FeedbackTypeName)
                .ToList();

            return Ok(sorted);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("PerFacultyFeedbackSummary")]
        public async Task<IActionResult> PerFacultyFeedbackSummary([FromQuery] string courseType, [FromQuery] int courseId, [FromQuery] string feedbackTypeIds )
        {
            try
            {
                var feedbackTypeIdList = new List<int>();

                if (!string.IsNullOrWhiteSpace(feedbackTypeIds))
                {
                    foreach (var id in feedbackTypeIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(id, out var parsed))
                        {
                            feedbackTypeIdList.Add(parsed);
                        }
                    }
                }

                var staffRatings = await (from fa in _context.FeedbackAnswers
                                          join fq in _context.FeedbackQuestions on fa.question_id equals fq.question_id
                                          join fs in _context.FeedbackSubmits on fa.feedback_submit_id equals fs.feedback_submit_id
                                          join f in _context.Feedback on fs.feedback_id equals f.FeedbackId
                                          join fg in _context.FeedbackGroup on fs.feedback_group_id equals fg.FeedbackGroupId
                                          join s in _context.Staff on fg.StaffId equals s.staff_id
                                          where (fq.question_type == "mcq" || fq.question_type == "rating")
                                                && f.course_id == courseId
                                                && feedbackTypeIdList.Contains(f.feedback_type_id)
                                                && f.Course.course_type == courseType
                                          select new
                                          {
                                              StaffId = s.staff_id,
                                              StaffName = s.first_name + " " + s.last_name,
                                              QuestionType = fq.question_type,
                                              AnswerValue = fa.answer
                                          }).ToListAsync();

                var mappedRatings = staffRatings.Select(x => new
                {
                    x.StaffId,
                    x.StaffName,
                    Value = x.QuestionType == "mcq"
                        ? MapMcqAnswerToNumber(x.AnswerValue)
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
                    .OrderByDescending(r => r.AverageRating)
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating faculty summary", details = ex.Message });
            }
        }

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

        [Authorize(Roles = "Admin")]
        [HttpPost("FacultyFeedbackSummary")]
        public async Task<IActionResult> FacultyFeedbackSummary([FromBody] FacultyFeedbackSummaryDto request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request" });

            // 1. Get the feedback type (if required)
            var feedbackType = await _context.FeedbackType
                .FirstOrDefaultAsync(ft => ft.feedback_type_title == request.type_name);

            if (feedbackType == null)
                return NotFound(new { message = "Feedback type not found." });

            // 2. Get matching feedback groups (same as you had)
            var feedbackGroups = await (
                from fg in _context.FeedbackGroup
                join f in _context.Feedback on fg.FeedbackId equals f.FeedbackId
                join c in _context.Courses on f.course_id equals c.course_id
                join m in _context.Modules on f.module_id equals m.module_id
                join s in _context.Staff on fg.StaffId equals s.staff_id
                where c.course_name == request.course_name
                      && m.module_name == request.module_name
                      && (s.first_name + " " + s.last_name) == request.staff_name
                select fg
            ).ToListAsync();

            if (!feedbackGroups.Any())
                return NotFound(new { message = "No feedback groups found." });

            var feedbackGroupIds = feedbackGroups.Select(fg => fg.FeedbackGroupId).ToList();

            // --- compute total students / submitted / remaining as you already do ---
            var firstFeedbackGroup = await _context.FeedbackGroup
                .Include(fg => fg.Feedback)
                .FirstOrDefaultAsync(fg => feedbackGroupIds.Contains(fg.FeedbackGroupId));

            int totalStudents = 0;
            if (firstFeedbackGroup != null)
            {
                int courseId = firstFeedbackGroup.Feedback.course_id;

                var courseStudents = from cs in _context.CourseStudents
                                     join st in _context.Students on cs.student_rollno equals st.student_rollno
                                     where cs.course_id == courseId
                                     select st;

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

            // 3. Fetch all answers with question info AND feedback_submit_id (important)
            var answers = await (from fa in _context.FeedbackAnswers
                                 join fq in _context.FeedbackQuestions on fa.question_id equals fq.question_id
                                 join fs in _context.FeedbackSubmits on fa.feedback_submit_id equals fs.feedback_submit_id
                                 where feedbackGroupIds.Contains(fs.feedback_group_id ?? 0)
                                       && fq.feedback_type_id == feedbackType.feedback_type_id
                                 select new
                                 {
                                     SubmitId = fs.feedback_submit_id,
                                     fq.question_id,
                                     fq.question,
                                     fq.question_type,
                                     Answer = fa.answer
                                 }).ToListAsync();

            // Local helper that maps answer -> numeric score (1..5) or null if cannot map
            int? MapAnswerToNumber(string answer, string questionType)
            {
                if (string.IsNullOrWhiteSpace(answer)) return null;

                // Normalize
                var a = answer.Trim();

                if (!string.IsNullOrEmpty(questionType) && questionType.Equals("mcq", StringComparison.OrdinalIgnoreCase))
                {
                    // Map common MCQ labels to numbers
                    return a.ToLowerInvariant() switch
                    {
                        "excellent" => 5,
                        "good" => 4,
                        "average" => 3,
                        "poor" => 2,
                        "very poor" => 1,
                        "verypoor" => 1,
                        _ => TryParseNumeric(a)
                    };
                }

                // If rating type or numeric answer, try parse number (allow decimals)
                return TryParseNumeric(a);
            }

            int? TryParseNumeric(string s)
            {
                if (int.TryParse(s, out var iv)) return iv;
                if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var dv))
                {
                    // return rounded integer or keep precise? We'll use the numeric value (as double) when averaging,
                    // but for counts we use rounded value. For Map we return rounded int.
                    return (int)Math.Round(dv);
                }
                return null;
            }

            // 4. Compute per-submission averages (ignore answers which couldn't be mapped)
            var submissionAverages = answers
                .GroupBy(a => a.SubmitId)
                .Select(g =>
                {
                    var mapped = g.Select(a => MapAnswerToNumber(a.Answer, a.question_type))
                                  .Where(n => n.HasValue)
                                  .Select(n => (double)n.Value)
                                  .ToList();

                    return mapped.Any() ? (double?)mapped.Average() : null;
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToList();

            double overallAvgRating = 0;
            if (submissionAverages.Any())
            {
                overallAvgRating = Math.Round(submissionAverages.Average(), 2); // round to 2 decimals
            }

            // 5. Build question stats (counts per answer category) — robust to numeric or label answers
            bool IsNumericEqual(string ans, int value)
            {
                if (int.TryParse(ans, out var iv)) return iv == value;
                if (double.TryParse(ans, NumberStyles.Any, CultureInfo.InvariantCulture, out var dv))
                    return (int)Math.Round(dv) == value;
                return false;
            }

            var questionStats = answers
                .GroupBy(a => new { a.question_id, a.question, a.question_type })
                .Select(g =>
                {
                    var qlist = g.ToList();
                    var excellent = qlist.Count(a => a.Answer != null &&
                        (a.Answer.Equals("Excellent", StringComparison.OrdinalIgnoreCase) || IsNumericEqual(a.Answer, 5)));
                    var good = qlist.Count(a => a.Answer != null &&
                        (a.Answer.Equals("Good", StringComparison.OrdinalIgnoreCase) || IsNumericEqual(a.Answer, 4)));
                    var average = qlist.Count(a => a.Answer != null &&
                        (a.Answer.Equals("Average", StringComparison.OrdinalIgnoreCase) || IsNumericEqual(a.Answer, 3)));
                    var poor = qlist.Count(a => a.Answer != null &&
                        (a.Answer.Equals("Poor", StringComparison.OrdinalIgnoreCase) || IsNumericEqual(a.Answer, 2)));
                    var veryPoor = qlist.Count(a => a.Answer != null &&
                        (a.Answer.Equals("Very Poor", StringComparison.OrdinalIgnoreCase) || IsNumericEqual(a.Answer, 1)));

                    return new
                    {
                        QuestionId = g.Key.question_id,
                        QuestionText = g.Key.question,
                        QuestionType = g.Key.question_type,
                        Excellent = excellent,
                        Good = good,
                        Average = average,
                        Poor = poor,
                        VeryPoor = veryPoor
                    };
                })
                .ToList();

            // 6. Return response (use lowercase property names if your front-end expects that)
            return Ok(new
            {
                staff_name = request.staff_name,
                module_name = request.module_name,
                course_name = request.course_name,
                type_name = request.type_name,
                date = request.date,
                submitted = submittedCount,
                remaining = remainingCount,
                rating = overallAvgRating,
                questions = questionStats.Select(q => new
                {
                    questionId = q.QuestionId,
                    questionText = q.QuestionText,
                    questionType = q.QuestionType,
                    excellent = q.Excellent,
                    good = q.Good,
                    average = q.Average,
                    poor = q.Poor,
                    veryPoor = q.VeryPoor
                })
            });
        }
    }

        
}
