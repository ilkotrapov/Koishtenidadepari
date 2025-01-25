using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery_System__Team_Enif_.Data;

namespace Delivery_System__Team_Enif_.Controllers
{
    public class PackageController : Controller
    {
        private readonly ProjectDbContext _context;

        public PackageController(ProjectDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePackage([FromBody] GPackage package)
        {
            if (ModelState.IsValid)
            {
                _context.Add(package);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
             return View(package);
        }

        [HttpGet("track/{trackingNumber}")]
        public async Task<IActionResult> TrackPackage(string trackingNumber)
        {
            var package = await _context.Packages.FirstOrDefaultAsync(p => p.Status == trackingNumber);
            if (package == null) return NotFound("Package not found.");

            return Ok(package);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePackageStatus(int id, [FromBody] string newStatus)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null) return NotFound();

            package.Status = newStatus;
            try
            {
                _context.Update(package);
                await _context.SaveChangesAsync();
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(package.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
        }


        // GET: Packages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Packages.ToListAsync());
        }

        // GET: Packages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // GET: Packages/Create
        public IActionResult Create()
        {
            return View();
        }
        // GET: Packages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package != null)
            {
                _context.Packages.Remove(package);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
            return _context.Packages.Any(e => e.Id == id);
        }
    }
}
