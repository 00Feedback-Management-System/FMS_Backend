using Feedback_System.Data;
using Feedback_System.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                // Step 1: build a "raw" projection that includes all necessary fields.
                // Use DefaultIfEmpty() joins (left joins) so missing answers/questions won't break.
                var raw = await (
                    from f in _context.Feedback
                    join c in _context.Courses on f.course_id equals c.course_id
                    join ft in _context.FeedbackType on f.feedback_type_id equals ft.feedback_type_id
                    join fs in _context.FeedbackSubmits on f.FeedbackId equals fs.feedback_id into fsj
                    from fs in fsj.DefaultIfEmpty()
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
                        // 'group' is the property name on FeedbackType; use @ to be safe
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
                        // rating answers stored as numbers in your data
                        if (double.TryParse(answer, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
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
                            case "poor": return 1.0;
                            // add synonyms if needed
                            default: return 0.0;
                        }
                    }

                    // descriptive or unknown types -> not included in rating
                    return 0.0;
                }

                // Step 2: group by Course + FeedbackType + GroupType
                var grouped = raw
                    .GroupBy(x => new { x.CourseId, x.CourseName, x.FeedbackTypeId, x.FeedbackTypeName, Group = x.GroupType })
                    .Select(g => {
                        // sessions: sum of distinct feedback.session values (avoid double counting due to multiple answers)
                        var sessionsSum = g
                            .GroupBy(x => x.FeedbackId)         // group by feedback id
                            .Select(gr => gr.First().Session)   // pick one session value per feedback
                            .Sum();

                        // date: max end_date among feedbacks in this group
                        var maxDate = g.Max(x => x.FeedbackEndDate);

                        // rating calculation: map each row to numeric score depending on question type, ignore zeros
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

                // Optionally: sort results (e.g., by CourseName then FeedbackTypeName)
                var sorted = grouped
                    .OrderBy(x => x.CourseName)
                    .ThenBy(x => x.FeedbackTypeName)
                    .ToList();

                return Ok(sorted);
            }
        }

    
}
