using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Feedback_System.Services;
using Microsoft.AspNetCore.Mvc;

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
    }
}
