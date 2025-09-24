using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;


namespace Feedback_System.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CourseController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/course
        [Authorize(Roles = "Admin")]
        [Route("api/GetAllCourse")]
        [HttpGet]
        public IActionResult GetCourses()
        {
            try
            {
                var courses = _context.Courses
                    .Select(c => new CourseDTO
                    {
                        course_id = c.course_id,
                        course_name = c.course_name,
                        start_date = c.start_date,
                        end_date = c.end_date,
                        duration = c.duration,
                        course_type = c.course_type
                    }).ToList();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while getting the courses.");
            }
        }

        //  POST: api/course
        // [Route("api/course")]
        //[HttpPost]


        //public IActionResult CreateCourse([FromBody] CourseDTO dto)
        //{
        //    if (dto == null)
        //        return BadRequest("Invalid data.");



        //    var course = new Course
        //    {
        //        course_name = dto.course_name,
        //        start_date = dto.start_date,
        //        end_date = dto.end_date,
        //        duration = dto.duration,
        //        course_type = dto.course_type
        //    };

        //    _context.Courses.Add(course);
        //    _context.SaveChanges();

        //    var response = new
        //    {
        //        message = "Course added successfully!",
        //        data = dto
        //    };

        //    dto.course_id = course.course_id; // return generated ID
        //    return CreatedAtAction(nameof(GetCourses), new { id = dto.course_id }, response);
        //}


        // POST: api/course
        [Authorize(Roles = "Admin")]
        [Route("api/AddCourse")]
        [HttpPost]
        public IActionResult CreateCourse([FromBody] CourseDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            // Field-level validation
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.course_name))
                errors.Add("Course name is required.");

            if (dto.start_date == default)
                errors.Add("Start date is required.");

            if (dto.end_date == default)
                errors.Add("End date is required.");

            if (dto.end_date < dto.start_date)
                errors.Add("End date cannot be before start date.");

            if (dto.duration <= 0)
                errors.Add("Duration must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.course_type))
                errors.Add("Course type is required.");

            if (errors.Count > 0)
                return BadRequest(new { errors });

            try
            {
                var course = new Course
                {
                    course_name = dto.course_name,
                    start_date = dto.start_date,
                    end_date = dto.end_date,
                    duration = dto.duration,
                    course_type = dto.course_type
                };

                _context.Courses.Add(course);
                _context.SaveChanges();

                dto.course_id = course.course_id; // return generated ID

                var response = new
                {
                    message = "Course added successfully!",
                    data = dto
                };

                return CreatedAtAction(nameof(GetCourses), new { id = dto.course_id }, response);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, "An error occurred while creating the course.");
            }
        }


        // PUT: api/course/{id}
        [Authorize(Roles = "Admin")]
        [Route("api/UpdateCourse/{id}")]

        [HttpPut]

        public IActionResult UpdateCourse(int id, [FromBody] CourseDTO dto)
        {
            if (dto == null || id != dto.course_id)
                return BadRequest("Invalid data.");
            var course = _context.Courses.Find(id);
            if (course == null)
                return NotFound("Course not found.");
            // Field-level validation
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.course_name))
                errors.Add("Course name is required.");
            if (dto.start_date == default)
                errors.Add("Start date is required.");
            if (dto.end_date == default)
                errors.Add("End date is required.");
            if (dto.end_date < dto.start_date)
                errors.Add("End date cannot be before start date.");
            if (dto.duration <= 0)
                errors.Add("Duration must be greater than zero.");
            if (string.IsNullOrWhiteSpace(dto.course_type))
                errors.Add("Course type is required.");
            if (errors.Count > 0)
                return BadRequest(new { errors });
            try
            {
                course.course_name = dto.course_name;
                course.start_date = dto.start_date;
                course.end_date = dto.end_date;
                course.duration = dto.duration;
                course.course_type = dto.course_type;
                _context.SaveChanges();
                return Ok(new { message = "Course updated successfully!", data = dto });
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, "An error occurred while updating the course.");
            }
        }


        // DELETE: api/course/{id}
        [Authorize(Roles = "Admin")]
        [Route("api/DeleteCourse/{id}")]
        [HttpDelete]
        public IActionResult DeleteCourse(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
                return NotFound("Course not found.");
            try
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
                return Ok(new { message = "Course deleted successfully!" });
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, "An error occurred while deleting the course.");
            }
        }


        // GET: api/course/{id}

        [Authorize(Roles = "Admin")]
        [Route("api/GetCourseById/{id}")]

        [HttpGet]
        public IActionResult GetCourseById(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
                return NotFound("Course not found.");
            try
            {
                var dto = new CourseDTO
                {
                    course_id = course.course_id,
                    course_name = course.course_name,
                    start_date = course.start_date,
                    end_date = course.end_date,
                    duration = course.duration,
                    course_type = course.course_type
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, "An error occurred while retrieving the course.");

            }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("api/GetCourseTypes")]
        public async Task<IActionResult> GetCourseTypes()
        {
            try
            {
                var types = await _context.Courses
                    .Select(c => c.course_type)
                    .Distinct()
                    .ToListAsync();

                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching course types", details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("api/GetCoursesByType/{type}")]
        [HttpGet]
        public async Task<IActionResult> GetCoursesByType(string type)
        {
            try
            {
                var courses = await _context.Courses
                                .Where(c => c.course_type.ToLower() == type.ToLower())
                                .Select(c => new CourseDTO
                                {
                                    course_id = c.course_id,
                                    course_name = c.course_name,
                                    start_date = c.start_date,
                                    end_date = c.end_date,
                                    duration = c.duration,
                                    course_type = c.course_type
                                })
                                .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching courses by type", details = ex.Message });
            }
        }
    }
}
