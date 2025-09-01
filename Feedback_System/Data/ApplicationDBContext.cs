using Feedback_System.Model;
using Microsoft.EntityFrameworkCore;

namespace Feedback_System.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options) { }

        public DbSet<Staff> Staff {  get; set; }
        public DbSet<Staffroles> Staffroles { get; set; }
        public DbSet<Groups> Groups { get; set; }
    }
}
