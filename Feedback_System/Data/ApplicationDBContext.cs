using Microsoft.EntityFrameworkCore;

namespace Feedback_System.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options) { }


    }
}
