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

            // ---------------- STUDENT LOGIN ----------------
            var student = _context.Students.FirstOrDefault(s => s.email == loginDto.email);
            if (student != null)
            {
                if (passwordHasher.VerifyHashedPassword(null, student.password, loginDto.password) == PasswordVerificationResult.Failed)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                var token = GenerateJwtToken(student.student_rollno.ToString(), student.email, "student");

                string? imageUrl = null;
                if (!string.IsNullOrEmpty(student.profile_image))
                {
                    imageUrl = $"{Request.Scheme}://{Request.Host}{student.profile_image}";
                }

                return Ok(new
                {
                    message = "Login successful.",
                    token,
                    users = new
                    {
                        id = student.student_rollno,
                        first_name = student.first_name,
                        last_name = student.last_name,
                        email = student.email,
                        role = "student",
                        image = imageUrl
                    }
                });
            }

            // ---------------- STAFF LOGIN ----------------
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

                string? imageUrl = null;
                if (!string.IsNullOrEmpty(staff.profile_image))
                {
                    imageUrl = $"{Request.Scheme}://{Request.Host}{staff.profile_image}";
                }

                return Ok(new
                {
                    message = "Login successful.",
                    token,
                    users = new
                    {
                        id = staff.staff_id,
                        first_name = staff.first_name,
                        last_name = staff.last_name,
                        email = staff.email,
                        role = roleName,
                        image = imageUrl
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
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

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
