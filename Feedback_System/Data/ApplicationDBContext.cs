using Feedback_System.Model;
using Microsoft.EntityFrameworkCore;

namespace Feedback_System.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options) { }


        public DbSet<Modules> Modules { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<FeedbackType> FeedbackType { get; set; }
        public DbSet<FeedbackQuestion> FeedbackQuestions { get; set; }
        public DbSet<FeedbackSubmit> FeedbackSubmits { get; set; }
        public DbSet<FeedbackAnswer> FeedbackAnswers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }
        public DbSet<RoleAccess> RoleAccesses { get; set; }
        public DbSet<Staff> Staff { get; set; }

        public DbSet<Staffroles> Staffroles { get; set; }
    }
}
