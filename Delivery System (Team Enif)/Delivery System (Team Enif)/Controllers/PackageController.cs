using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery_System__Team_Enif_.Data;
using Delivery_System__Team_Enif_.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Delivery_System__Team_Enif_.Controllers
{
    public class PackageController : Controller
    {
        private readonly ProjectDbContext _projectDbContext;

        public PackageController(ProjectDbContext projectDbContext)
        {
            this._projectDbContext = projectDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Package> packages = _projectDbContext.Packages
                .Include(p => p.DeliveryOption)
                .Include(p => p.DeliveryType)
                .Include(p => p.DeliveryStatus)
                .ToList();
            PackageViewModel viewModel = new PackageViewModel
            {
                Packages = packages
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new PackageViewModel()
            {
                DeliveryOptions = Enum.GetValues(typeof(DeliveryOptionEnum))
                                    .Cast<DeliveryOptionEnum>()
                                    .Select(o => new SelectListItem
                                    {
                                        Value = ((int)o).ToString(),
                                        Text = o.ToString(),
                                        Selected = o == DeliveryOptionEnum.PickUp_DropOffLocalOffice
                                    }
                                    ).ToList(),
                DeliveryTypes = Enum.GetValues(typeof(DeliveryTypeEnum))
                                    .Cast<DeliveryTypeEnum>()
                                    .Select(t => new SelectListItem
                                    {
                                        Value = ((int)t).ToString(),
                                        Text = t.ToString(),
                                        Selected = t == DeliveryTypeEnum.Standard
                                    }
                                    ).ToList(),
                DeliveryStatuses = Enum.GetValues(typeof(DeliveryStatusEnum))
                                    .Cast<DeliveryStatusEnum>()
                                    .Select(s => new SelectListItem
                                    {
                                        Value = ((int)s).ToString(),
                                        Text = s.ToString(),
                                        Selected = s == DeliveryStatusEnum.Pending
                                    }
                                    ).ToList(),
                DeliveryDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateConfirm(Package package)
        {
                package.DeliveryStatusId = (int)DeliveryStatusEnum.Pending;
                _projectDbContext.Add(package);
                _projectDbContext.SaveChanges();

                return RedirectToAction("Index");
        }

        [HttpGet("track/{trackingNumber}")]
        public async Task<IActionResult> TrackPackage(int trackingNumber)
        {
            var package = await _projectDbContext.Packages.FirstOrDefaultAsync(p => p.Id == trackingNumber);
            if (package == null) return NotFound("Package not found.");

            return Ok(package);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePackageStatus(int id, [FromBody] int newStatus)
        {
            var package = await _projectDbContext.Packages.FindAsync(id);
            if (package == null) return NotFound();

            package.DeliveryStatusId = newStatus;
            try
            {
                _projectDbContext.Update(package);
                await _projectDbContext.SaveChangesAsync();
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
                return NotFound("The provided package id is null");
            }

            var package = await _projectDbContext.Package
                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return NotFound("No package with provided package id");
            }

            PackageViewModel viewModel = new PackageViewModel
            {
                Id = package.Id,
                SenderName = package.SenderName,
                SenderAddress = package.SenderAddress,
                RecipientName = package.RecipientName,
                RecipientAddress = package.RecipientAddress,
                Length = package.Length,
                Width = package.Width,
                Hight = package.Hight,
                Weight = package.Weight,
                DeliveryOptionSelected = (DeliveryOptionEnum)Enum.ToObject(typeof(DeliveryOptionEnum), package.DeliveryOptionId),
                DeliveryTypeSelected = (DeliveryTypeEnum)Enum.ToObject(typeof(DeliveryTypeEnum), package.DeliveryTypeId),
                DeliveryStatusSelected = (DeliveryStatusEnum)Enum.ToObject(typeof(DeliveryStatusEnum), package.DeliveryStatusId),
                DeliveryDate = package.DeliveryDate
            }; 

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound("The provided package id is null");
            }

            var package = await _projectDbContext.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound("No package found with the provided package id");
            }

            if (package.DeliveryStatusId != (int)DeliveryStatusEnum.Pending)
            {
                return BadRequest("The package is not editable!");
            }

            var deliveryOptions = Enum.GetValues(typeof(DeliveryOptionEnum))
                                    .Cast<DeliveryOptionEnum>()
                                    .Select(t => new SelectListItem
                                    {
                                        Value = ((int)t).ToString(),
                                        Text = t.ToString(),
                                        Selected = (int)t == package.DeliveryOptionId
                                    }
                                    ).ToList();
            var deliveryStatuses = Enum.GetValues(typeof(DeliveryStatusEnum))
                               .Cast<DeliveryStatusEnum>()
                               .Select(s => new SelectListItem
                               {
                                   Value = ((int)s).ToString(),
                                   Text = s.ToString(),
                                   Selected = (int)s == package.DeliveryStatusId
                               }).ToList();

            var deliveryTypes = Enum.GetValues(typeof(DeliveryTypeEnum))
                                    .Cast<DeliveryTypeEnum>()
                                    .Select(t => new SelectListItem
                                    {
                                        Value = ((int)t).ToString(),
                                        Text = t.ToString(),
                                        Selected = (int)t == package.DeliveryTypeId
                                    }).ToList();

            PackageViewModel viewModel = new PackageViewModel
            {
                Id = package.Id,
                SenderName = package.SenderName,
                RecipientName = package.RecipientName,
                SenderAddress = package.SenderAddress,
                RecipientAddress = package.RecipientAddress,

                Length = package.Length,
                Width = package.Width,
                Hight = package.Hight,

                Weight = package.Weight,
                DeliveryOptionId = package.DeliveryOptionId,
                DeliveryOptions = deliveryOptions,
                DeliveryTypeId = package.DeliveryTypeId,
                DeliveryTypes = deliveryTypes,
                DeliveryStatusId = package.DeliveryStatusId,
                DeliveryStatuses = deliveryStatuses,
                DeliveryDate = package.DeliveryDate
            };

            return View(viewModel);
        }

        // POST: Confirm Package Edit
        [HttpPost]
        public async Task<IActionResult> Edit(PackageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var package = await _projectDbContext.Packages.FindAsync(viewModel.Id);
            if (package == null)
            {
                return NotFound("No package found to edit");
            }

            package.SenderName = viewModel.SenderName;
            package.RecipientName = viewModel.RecipientName;
            package.SenderAddress = viewModel.SenderAddress;
            package.RecipientAddress = viewModel.RecipientAddress;
            package.Length = viewModel.Length;
            package.Width = viewModel.Width;
            package.Hight = viewModel.Hight;
            package.Weight = viewModel.Weight;

            var doe = _projectDbContext.DeliveryOptions.FirstOrDefault(s => s.Id == viewModel.DeliveryOptionId);
            if (doe != null)
            {
                package.DeliveryOptionId = doe.Id;
            }

            var dte = _projectDbContext.DeliveryOptions.FirstOrDefault(s => s.Id == viewModel.DeliveryTypeId);
            if (dte != null)
            {
                package.DeliveryTypeId = dte.Id;
            }

            package.DeliveryDate = viewModel.DeliveryDate;

            await _projectDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        // GET: Packages/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound("The provided package id is null");
            }

            var package = await _projectDbContext.Package
                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return NotFound("No package with provided package id");
            }

            if (package.DeliveryStatusId != (int)DeliveryStatusEnum.Pending)
            {
                return BadRequest("The package ca not be deleted!");
            }

            PackageViewModel viewModel = new PackageViewModel
            {
                Id = package.Id,
                SenderName = package.SenderName,
                RecipientName = package.RecipientName,
                DeliveryOptionSelected = (DeliveryOptionEnum)Enum.ToObject(typeof(DeliveryOptionEnum), package.DeliveryOptionId),
                DeliveryTypeSelected = (DeliveryTypeEnum)Enum.ToObject(typeof(DeliveryTypeEnum), package.DeliveryTypeId),
                DeliveryStatusSelected = (DeliveryStatusEnum)Enum.ToObject(typeof(DeliveryStatusEnum), package.DeliveryStatusId),
                DeliveryDate = package.DeliveryDate
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var package = await _projectDbContext.Packages.FindAsync(id);
            if (package != null)
            {
                _projectDbContext.Packages.Remove(package);
            }

            await _projectDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
            return _projectDbContext.Packages.Any(e => e.Id == id);
        }
    }
}
