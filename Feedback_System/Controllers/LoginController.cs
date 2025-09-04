using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Feedback_System.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly PasswordServices _passwordServices;

        public LoginController(ApplicationDBContext context, PasswordServices passwordServices)
        {
            _context = context;
            _passwordServices = passwordServices;
        }

        [Route("api/Login")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto.role == null)
            {
                return BadRequest("Role is required.");
            }

            var passwordHasher = new PasswordHasher<object>();

            if (loginDto.role == "student")
            {
                var student = _context.Students.FirstOrDefault(s => s.email == loginDto.email);

                if (student == null ||
                    passwordHasher.VerifyHashedPassword(null, student.password, loginDto.password) == PasswordVerificationResult.Failed)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                return Ok(new
                {
                    message = "Login successful.",
                    user = new
                    {
                        id = student.student_rollno,
                        first_name = student.first_name,
                        last_name = student.last_name,
                        email = student.email,
                        role = "student"
                    }
                });
            }
            else if (loginDto.role == "staff")
            {
                var user = _context.Staff.FirstOrDefault(s => s.email == loginDto.email);

                if (user == null ||
                    passwordHasher.VerifyHashedPassword(null, user.password, loginDto.password) == PasswordVerificationResult.Failed)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                // 🔎 fetch role name using staffrole_id
                var roleName = _context.Staffroles
                                .Where(r => r.staffrole_id == user.staffrole_id)
                                .Select(r => r.staffrole_name)
                                .FirstOrDefault();
                //if(roleName != "Trainer")
                //{
                //    return Unauthorized(new { message = "Access denied. Not an staff user." });
                //}

                return Ok(new
                {
                    message = "Login successful.",
                    users = new
                    {
                        id = user.staff_id,
                        first_name = user.first_name,
                        last_name = user.last_name,
                        email = user.email,
                        //role = user.staffrole_id
                        role = roleName
                    }
                });
            }
            else if (loginDto.role == "admin")
            {
                var user = _context.Staff.FirstOrDefault(s => s.email == loginDto.email);
                if (user == null ||
                    passwordHasher.VerifyHashedPassword(null, user.password, loginDto.password) == PasswordVerificationResult.Failed)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }
                // 🔎 fetch role name using staffrole_id
                var roleName = _context.Staffroles
                                .Where(r => r.staffrole_id == user.staffrole_id)
                                .Select(r => r.staffrole_name)
                                .FirstOrDefault();
                if (roleName != "Admin")
                {
                    return Unauthorized(new { message = "Access denied. Not an admin user." });
                }
                return Ok(new
                {
                    message = "Login successful.",
                    users = new
                    {
                        id = user.staff_id,
                        first_name = user.first_name,
                        last_name = user.last_name,
                        email = user.email,
                        //role = user.staffrole_id
                        role = roleName
                    }
                });
            }
            else
            {
                return BadRequest("Invalid role specified.");
            }
        }


        [HttpPost]
        [Route("api/Forgot-Password")]

        public IActionResult ForgotPassword([FromBody] LoginDto loginDto)
        {
            if (loginDto.role == null)
            {
                return BadRequest("Role is required.");
            }
            var passwordHasher = new PasswordHasher<object>();
            if (loginDto.role == "student")
            {
                var student = _context.Students.FirstOrDefault(s => s.email == loginDto.email);
                if (student == null)
                {
                    return NotFound(new { message = "Email not found." });
                }
                // Hash the new password
                var hashedPassword = passwordHasher.HashPassword(null, loginDto.password);
                student.password = hashedPassword;
                _context.SaveChanges();
                return Ok(new { message = "Password reset successful." });
            }
            else if (loginDto.role == "staff")
            {
                var user = _context.Staff.FirstOrDefault(s => s.email == loginDto.email);
                if (user == null)
                {
                    return NotFound(new { message = "Email not found." });
                }
                // Hash the new password
                var hashedPassword = passwordHasher.HashPassword(null, loginDto.password);
                user.password = hashedPassword;
                _context.SaveChanges();
                return Ok(new { message = "Password reset successful." });
            }
            else if (loginDto.role == "admin")
            {
                var user = _context.Staff.FirstOrDefault(s => s.email == loginDto.email);
                if (user == null)
                {
                    return NotFound(new { message = "Email not found." });
                }
                // Hash the new password
                var hashedPassword = passwordHasher.HashPassword(null, loginDto.password);
                user.password = hashedPassword;
                _context.SaveChanges();
                return Ok(new { message = "Password reset successful." });
            }
            else
            {
                return BadRequest("Invalid role specified.");
            }
        }
    }
}
