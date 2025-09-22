using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feedback_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public GroupsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Groups>>> GetGroups()
        {
            return await _context.Groups.ToListAsync();
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Groups>> GetGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            return group;
        }

        [HttpGet("ByCourse/{courseId}")]
        public async Task<ActionResult<IEnumerable<Groups>>> GetGroupsByCourse(int courseId)
        {
            if (courseId <= 0)
                return BadRequest("Invalid course id.");

            var groups = await _context.CourseGroups
                .Include(cg => cg.Groups)
                .Where(cg => cg.course_id == courseId)
                .Select(cg => cg.Groups)
                .ToListAsync();

           
            return Ok(groups); // returns [] if no groups
        }

        // POST: api/CourseGroup/addGroups
        [HttpPost("addGroups")]
        public async Task<IActionResult> AddGroups([FromBody] AddGroupsDto dto)
        {
            if (dto == null || dto.course_id <= 0 || dto.groups == null || !dto.groups.Any())
            {
                return BadRequest("Invalid input data.");
            }

            // Check if course exists
            var course = await _context.Courses.FindAsync(dto.course_id);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            // Delete existing groups for the course
            var existingCourseGroups = await _context.CourseGroups
                .Where(cg => cg.course_id == dto.course_id)
                .ToListAsync();

            if (existingCourseGroups.Any())
            {
                // Get group IDs to delete
                var existingGroupIds = existingCourseGroups.Select(cg => cg.group_id).ToList();

                // Delete mappings first
                _context.CourseGroups.RemoveRange(existingCourseGroups);

                // Delete groups
                var groupsToDelete = await _context.Groups
                    .Where(g => existingGroupIds.Contains(g.group_id))
                    .ToListAsync();
                _context.Groups.RemoveRange(groupsToDelete);

                await _context.SaveChangesAsync();
            }

            // Add new groups and mapping
            var createdGroups = new List<Groups>();
            foreach (var groupName in dto.groups)
            {
                if (string.IsNullOrWhiteSpace(groupName)) continue;

                // Save Group
                var group = new Groups
                {
                    group_name = groupName
                };
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                createdGroups.Add(group);

                // Save mapping in CourseGroup
                var courseGroup = new CourseGroup
                {
                    course_id = dto.course_id,
                    group_id = group.group_id
                };
                _context.CourseGroups.Add(courseGroup);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Groups added/updated successfully",
                groups = createdGroups.Select(g => new { g.group_id, g.group_name })
            });
        }


        // PUT: api/Groups/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup(int id, Groups group)
        {
            if (id != group.group_id)
            {
                return BadRequest();
            }

            _context.Entry(group).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
