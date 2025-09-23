using Feedback_System.Data;
using Feedback_System.DTO;
using Feedback_System.Model;
using Feedback_System.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Feedback_System.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly PasswordServices _passwordServices;
        private readonly JwtSettings _jwtSettings;

        public LoginController(ApplicationDBContext context, PasswordServices passwordServices, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _passwordServices = passwordServices;
            _jwtSettings = jwtSettings.Value;
        }

        [Route("api/Login")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var passwordHasher = new PasswordHasher<object>();

            var student = _context.Students.FirstOrDefault(s => s.email == loginDto.email);
            if (student != null)
            {
                if (passwordHasher.VerifyHashedPassword(null, student.password, loginDto.password) == PasswordVerificationResult.Failed)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                var token = GenerateJwtToken(student.student_rollno.ToString(), student.email, "student");

                return Ok(new
                {
                    message = "Login successful.",
                    token = token,
                    users = new
                    {
                        id = student.student_rollno,
                        first_name = student.first_name,
                        last_name = student.last_name,
                        email = student.email,
                        role = "student"
                    }
                });
            }

            var staff = _context.Staff.FirstOrDefault(s => s.email == loginDto.email);
            if (staff != null)
            {
                if (passwordHasher.VerifyHashedPassword(null, staff.password, loginDto.password) == PasswordVerificationResult.Failed)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                var roleName = _context.Staffroles
                                .Where(r => r.staffrole_id == staff.staffrole_id)
                                .Select(r => r.staffrole_name)
                                .FirstOrDefault();

                if (string.IsNullOrEmpty(roleName))
                {
                    return Unauthorized(new { message = "Role not assigned for this staff." });
                }

                var token = GenerateJwtToken(staff.staff_id.ToString(), staff.email, roleName);

                return Ok(new
                {
                    message = "Login successful.",
                    token = token,
                    users = new
                    {
                        id = staff.staff_id,
                        first_name = staff.first_name,
                        last_name = staff.last_name,
                        email = staff.email,
                        role = roleName,
                    }
                });
            }

            return Unauthorized(new { message = "Invalid email or password." });
        }

        private string GenerateJwtToken(string userId, string email, string role)
        {
            var claims = new[]
                    {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("role", role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        //[HttpPost]
        //[Route("api/Forgot-Password")]

        //public IActionResult ForgotPassword([FromBody] LoginDto loginDto)
        //{
        //    if (loginDto.role == null)
        //    {
        //        return BadRequest("Role is required.");
        //    }
        //    var passwordHasher = new PasswordHasher<object>();
        //    if (loginDto.role == "student")
        //    {
        //        var student = _context.Students.FirstOrDefault(s => s.email == loginDto.email);
        //        if (student == null)
        //        {
        //            return NotFound(new { message = "Email not found." });
        //        }
        //        // Hash the new password
        //        var hashedPassword = passwordHasher.HashPassword(null, loginDto.password);
        //        student.password = hashedPassword;
        //        _context.SaveChanges();
        //        return Ok(new { message = "Password reset successful." });
        //    }
        //    else if (loginDto.role == "staff")
        //    {
        //        var user = _context.Staff.FirstOrDefault(s => s.email == loginDto.email);

        //        if (user == null ||
        //            passwordHasher.VerifyHashedPassword(null, user.password, loginDto.password) == PasswordVerificationResult.Failed)
        //        {
        //            return Unauthorized(new { message = "Invalid email or password." });
        //        }

        //        // fetch role name using staffrole_id
        //        var roleName = _context.Staffroles
        //                        .Where(r => r.staffrole_id == user.staffrole_id)
        //                        .Select(r => r.staffrole_name)
        //                        .FirstOrDefault();

        //        // ✅ fetch scheduled feedback for this staff
        //        var scheduledFeedbacks = _context.Feedback
        //            .Include(f => f.Course)
        //            .Include(f => f.Module)
        //            .Include(f => f.FeedbackType)
        //            .Where(f => f.FeedbackGroups.Any(fg => fg.StaffId == user.staff_id))
        //            .Select(f => new {
        //                f.FeedbackId,
        //                Course = f.Course.course_name,
        //                Module = f.Module.module_name,
        //                Type = f.FeedbackType.feedback_type_title,
        //                Date = f.start_date,
        //                Session = f.session
        //            })
        //            .ToList();

        //        return Ok(new
        //        {
        //            message = "Login successful.",
        //            users = new
        //            {
        //                id = user.staff_id,
        //                first_name = user.first_name,
        //                last_name = user.last_name,
        //                email = user.email,
        //                role = roleName,
        //                scheduledFeedback = scheduledFeedbacks
        //            }
        //        });
        //    }

        //    //else if (loginDto.role == "staff")
        //    //{
        //    //    var user = _context.Staff.FirstOrDefault(s => s.email == loginDto.email);
        //    //    if (user == null)
        //    //    {
        //    //        return NotFound(new { message = "Email not found." });
        //    //    }
        //    //    // Hash the new password
        //    //    var hashedPassword = passwordHasher.HashPassword(null, loginDto.password);
        //    //    user.password = hashedPassword;
        //    //    _context.SaveChanges();
        //    //    return Ok(new { message = "Password reset successful." });
        //    //}
        //    else if (loginDto.role == "admin")
        //    {
        //        var user = _context.Staff.FirstOrDefault(s => s.email == loginDto.email);
        //        if (user == null)
        //        {
        //            return NotFound(new { message = "Email not found." });
        //        }
        //        // Hash the new password
        //        var hashedPassword = passwordHasher.HashPassword(null, loginDto.password);
        //        user.password = hashedPassword;
        //        _context.SaveChanges();
        //        return Ok(new { message = "Password reset successful." });
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid role specified.");
        //    }
        //}

        //


        //newly added code for change password

        [HttpPost]
        [Route("api/Forgot-Password")]
        public IActionResult ForgotPassword([FromBody] LoginDto loginDto)
        {
            var passwordHasher = new PasswordHasher<object>();

            // 🔹 First check in Students table
            var student = _context.Students.FirstOrDefault(s => s.email == loginDto.email);
            if (student != null)
            {
                var hashedPassword = passwordHasher.HashPassword(null, loginDto.password);
                student.password = hashedPassword;
                _context.SaveChanges();

                return Ok(new { message = "Password reset successful (Student)." });
            }

            // 🔹 If not found, check in Staff table
            var staff = _context.Staff.FirstOrDefault(s => s.email == loginDto.email);
            if (staff != null)
            {
                var hashedPassword = passwordHasher.HashPassword(null, loginDto.password);
                staff.password = hashedPassword;
                _context.SaveChanges();

                return Ok(new { message = "Password reset successful (Staff)." });
            }

            // 🔹 If email not found in either table
            return Ok(new { message = "Invalid email." });
        }

    }
}
