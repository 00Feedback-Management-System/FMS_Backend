using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;

namespace Feedback_System.Controllers
{


    [Route("api/StudentApi")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly ApplicationDBContext _db;

        public StudentController(ApplicationDBContext db)
        {
            _db = db;
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
                        group_id = s.group_id,
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
                group_id = student.group_id,
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

            var student = new Student
            {
                student_rollno = studentDto.student_rollno,
                first_name = studentDto.first_name,
                last_name = studentDto.last_name,
                email = studentDto.email,
                password = studentDto.password,
                group_id = studentDto.group_id,
                profile_image = studentDto.profile_image,
                login_time = studentDto.login_time
            };

            _db.Students.Add(student);
            _db.SaveChanges();

            var createdDto = new StudentDto
            {
                student_rollno = student.student_rollno,
                first_name = student.first_name,
                last_name = student.last_name,
                email = student.email,
                password = student.password,
                group_id = student.group_id,
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
            studentFromDb.login_time = studentDto.login_time;

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

    }
}
