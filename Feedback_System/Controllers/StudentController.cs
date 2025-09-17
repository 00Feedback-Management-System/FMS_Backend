using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Feedback_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feedback_System.Controllers
{


    [Route("api/StudentApi")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly PasswordServices _passwordServices;

        public StudentController(ApplicationDBContext db, PasswordServices passwordServices)
        {
            _db = db;
            _passwordServices = passwordServices;
        }


        // GET: api/Studentapi
        [Route("GetStudent")]
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<StudentDto>> GetStudents()
        {
            try
            {
                var students = _db.Students
                    .Select(s => new StudentDto
                    {
                        student_rollno = s.student_rollno,
                        first_name = s.first_name,
                        last_name = s.last_name,
                        email = s.email,
                        password = s.password,
                        group_id = (int)s.group_id,
                        profile_image = s.profile_image,
                        login_time = s.login_time
                    })
                    .ToList();

                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetStudentByRollNo/{student_rollno}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<StudentDto> GetStudentbyrollno(int student_rollno)
        {
            //if (string.IsNullOrEmpty(student_rollno))
            //    return BadRequest("Student roll number is required.");

            var student = _db.Students.FirstOrDefault(u => u.student_rollno == student_rollno);

            if (student == null)
                return NotFound($"No student found with roll number '{student_rollno}'.");

            var studentDto = new StudentDto
            {
                student_rollno = student.student_rollno,
                first_name = student.first_name,
                last_name = student.last_name,
                email = student.email,
                password = student.password,
                group_id = (int)student.group_id,
                profile_image = student.profile_image,
                login_time = student.login_time
            };

            return Ok(studentDto);
        }



        // POST: api/Studentapi
        [Route("CreateStudent")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public ActionResult<StudentDto> CreateStudent([FromBody] StudentDto studentDto)
        {
            if (studentDto == null)
                return BadRequest("Student data is required.");

            if (_db.Students.Any(u => u.student_rollno == studentDto.student_rollno))
            {
                ModelState.AddModelError("student_rollno", "A student with this roll number already exists.");
                return BadRequest(ModelState);
            }
            var hashedPassword = _passwordServices.HashPassword(studentDto.password);

            var student = new Student
            {
              
                first_name = studentDto.first_name,
                last_name = studentDto.last_name,
                email = studentDto.email,
                password = hashedPassword,
                group_id = studentDto.group_id,
                profile_image = studentDto.profile_image,
                login_time = DateTime.Now
            };

            _db.Students.Add(student);
            _db.SaveChanges();
            studentDto.password = null;
            var createdDto = new StudentDto
            {
                student_rollno = student.student_rollno,
                first_name = student.first_name,
                last_name = student.last_name,
                email = student.email,
                password = student.password,
                group_id = (int)student.group_id,
                profile_image = student.profile_image,
                login_time = student.login_time
            };

            return CreatedAtAction(nameof(GetStudents),
                new { student_rollno = createdDto.student_rollno },
                createdDto);
        }

        [HttpPut("UpdateStudent/{student_rollno}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateStudent(int student_rollno, [FromBody] StudentDto studentDto)
        {
            if (studentDto == null || student_rollno != studentDto.student_rollno)
                return BadRequest("Invalid student data or roll number mismatch.");

            var studentFromDb = _db.Students.FirstOrDefault(u => u.student_rollno == student_rollno);
            if (studentFromDb == null)
                return NotFound($"No student found with roll number '{student_rollno}'.");


            studentFromDb.first_name = studentDto.first_name;
            studentFromDb.last_name = studentDto.last_name;
            studentFromDb.email = studentDto.email;
            studentFromDb.password = studentDto.password;
            studentFromDb.group_id = studentDto.group_id;
            studentFromDb.profile_image = studentDto.profile_image;
            studentFromDb.login_time = (DateTime)studentDto.login_time;

            _db.Students.Update(studentFromDb);
            _db.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Studentapi/{student_rollno}

        [HttpGet("DeleteStudent/{student_rollno}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteStudent(int student_rollno)
        {
            //if (string.IsNullOrWhiteSpace(student_rollno))
            //    return BadRequest("Student roll number is required.");

            var student = _db.Students.FirstOrDefault(u => u.student_rollno == student_rollno);
            if (student == null)
                return NotFound($"No student found with roll number '{student_rollno}'.");

            _db.Students.Remove(student);
            _db.SaveChanges();

            return NoContent();
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var students = _db.Students.ToList();
            var student = _db.Students.FirstOrDefault(u => u.email == loginDto.email);
            if (student == null)
                return Unauthorized("Invalid credentials");

            var isValid = _passwordServices.VerifyPassword(student.password, loginDto.password);
            if (!isValid)
                return Unauthorized("Invalid credentials");

            return Ok(new { message = "Login successful", student_rollno = student.student_rollno });
        }

        [HttpGet("Submitted/{feedbackGroupId}")]
        public async Task<IActionResult> GetSubmittedStudents(int feedbackGroupId)
        {
            if (feedbackGroupId <= 0)
                return BadRequest("Invalid Feedback Group Id");

            // Find feedback group
            var feedbackGroup = await _db.FeedbackGroup
                .FirstOrDefaultAsync(fg => fg.FeedbackGroupId == feedbackGroupId);

            if (feedbackGroup == null)
                return NotFound("Feedback group not found.");

            IQueryable<FeedbackSubmit> query = _db.FeedbackSubmits
                .Where(fs => fs.feedback_group_id == feedbackGroupId)
                .Include(fs => fs.Students)
                .ThenInclude(s => s.Groups);  // ✅ Include Group for group_name

            // ✅ If group exists → filter based on group
            if (feedbackGroup.GroupId.HasValue)
            {
                int groupId = feedbackGroup.GroupId.Value;
                query = query.Where(fs => fs.Students.group_id == groupId);
            }

            // Final projection with group_name
            var submittedStudents = await query
                .Select(fs => new
                {
                    fs.Students.student_rollno,
                    fs.Students.first_name,
                    fs.Students.last_name,
                    fs.Students.email,
                    GroupName = fs.Students.Groups.group_name   // ✅ group name
                })
                .ToListAsync();

            return Ok(submittedStudents);
        }


        [HttpGet("NotSubmitted/{feedbackGroupId}")]
        public async Task<IActionResult> GetNotSubmittedStudents(int feedbackGroupId)
        {
            if (feedbackGroupId <= 0)
                return BadRequest("Invalid Feedback Group Id");

            // Find feedback group with feedback details
            var feedbackGroup = await _db.FeedbackGroup
                .Include(fg => fg.Feedback)
                .FirstOrDefaultAsync(fg => fg.FeedbackGroupId == feedbackGroupId);

            if (feedbackGroup == null)
                return NotFound("Feedback group not found.");

            int courseId = feedbackGroup.Feedback.course_id;

            // Get all submitted student roll numbers for this feedback group
            var submittedStudentIds = await _db.FeedbackSubmits
                .Where(fs => fs.feedback_group_id == feedbackGroupId)
                .Select(fs => fs.student_rollno)
                .ToListAsync();

            // ✅ Start with CourseStudents and include Student + Group
            var courseStudentsQuery = _db.CourseStudents
                .Where(cs => cs.course_id == courseId)
                .Include(cs => cs.Student)
                .ThenInclude(s => s.Groups)  // ✅ Group included here
                .Select(cs => cs.Student);

            // If groupId exists, further filter by group
            if (feedbackGroup.GroupId.HasValue)
            {
                int groupId = feedbackGroup.GroupId.Value;
                courseStudentsQuery = courseStudentsQuery.Where(s => s.group_id == groupId);
            }

            // Exclude submitted students and project with GroupName
            var notSubmittedStudents = await courseStudentsQuery
                .Where(s => !submittedStudentIds.Contains(s.student_rollno))
                .Select(s => new
                {
                    s.student_rollno,
                    s.first_name,
                    s.last_name,
                    s.email,
                    GroupName = s.Groups.group_name   // ✅ group name here
                })
                .ToListAsync();

            return Ok(notSubmittedStudents);
        }



        [HttpGet("FeedbackSubmit/{feedbackGroupId}")]
        public async Task<IActionResult> GetFeedbackSummary(int feedbackGroupId)
        {
            var submittedCount = await _db.FeedbackSubmits
                .CountAsync(fs => fs.feedback_group_id == feedbackGroupId);

            // get courseId via feedback
            var feedbackGroup = await _db.FeedbackGroup
                .Include(fg => fg.Feedback)
                .FirstOrDefaultAsync(fg => fg.FeedbackGroupId == feedbackGroupId);

            if (feedbackGroup == null) return NotFound("FeedbackGroup not found");

            int courseId = feedbackGroup.Feedback.course_id;

            // total enrolled students for course (and group if exists)
            var courseStudents = _db.CourseStudents.Where(cs => cs.course_id == courseId).Select(cs => cs.Student);
            if (feedbackGroup.GroupId.HasValue)
            {
                int groupId = feedbackGroup.GroupId.Value;
                courseStudents = courseStudents.Where(s => s.group_id == groupId);
            }

            var totalStudents = await courseStudents.CountAsync();
            var remainingCount = totalStudents - submittedCount;

            return Ok(new { submittedCount, remainingCount });
        }





    }
}

