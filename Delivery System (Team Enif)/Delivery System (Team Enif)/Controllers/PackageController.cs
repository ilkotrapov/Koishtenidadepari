using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery_System__Team_Enif_.Data;
using Delivery_System__Team_Enif_.Data.Entities;

namespace Delivery_System__Team_Enif_.Controllers
{
    public class PackageController : Controller
    {
        private readonly ProjectDbContext projectDbContext;

        public PackageController(ProjectDbContext projectDbContext)
        {
            this.projectDbContext = projectDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Package> packages = projectDbContext.Packages.ToList();
            PackageViewModel viewModel = new PackageViewModel
            {
                Packages = packages
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateConfirm(Package package)
        {
                this.projectDbContext.Add(package);
                this.projectDbContext.SaveChanges();

                return RedirectToAction("Index");
        }

        [HttpGet("track/{trackingNumber}")]
        public async Task<IActionResult> TrackPackage(string trackingNumber)
        {
            var package = await projectDbContext.Packages.FirstOrDefaultAsync(p => p.Status == trackingNumber);
            if (package == null) return NotFound("Package not found.");

            return Ok(package);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePackageStatus(int id, [FromBody] string newStatus)
        {
            var package = await projectDbContext.Packages.FindAsync(id);
            if (package == null) return NotFound();

            package.Status = newStatus;
            try
            {
                projectDbContext.Update(package);
                await projectDbContext.SaveChangesAsync();
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

        // GET: Packages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await projectDbContext.Packages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        
        // GET: Packages/Delete/5

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await projectDbContext.Packages
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
            var package = await projectDbContext.Packages.FindAsync(id);
            if (package != null)
            {
                projectDbContext.Packages.Remove(package);
            }

            await projectDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
            return projectDbContext.Packages.Any(e => e.Id == id);
        }
    }
}
