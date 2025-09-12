using Feedback_System.Data;
using Feedback_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feedback_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController: ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ModulesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Modules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Modules>>> GetModules()
        {
            return await _context.Modules.ToListAsync();
        }

        // GET: api/Modules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Modules>> GetModule(int id)
        {
            var module = await _context.Modules.FindAsync(id);

            if (module == null)
            {
                return NotFound();
            }

            return module;
        }

        // ✅ Get all modules by courseId
        [HttpGet("ByCourse/{courseId}")]
        public async Task<ActionResult<IEnumerable<Modules>>> GetModulesByCourse(int courseId)
        {
            var modules = await _context.Modules
                .Where(m => m.course_id == courseId)
                .ToListAsync();

            if (modules == null || !modules.Any())
                return NotFound(new { message = "No modules found for this course." });

            return Ok(modules);
        }

        // POST: api/Modules
        [HttpPost]
        public async Task<ActionResult<Modules>> PostModule(Modules module)
        {
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetModule), new { id = module.module_id }, module);
        }

        // PUT: api/Modules/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModule(int id, Modules module)
        {
            if (id != module.module_id)
            {
                return BadRequest();
            }

            _context.Entry(module).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Modules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
            {
                return NotFound();
            }

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }


}
