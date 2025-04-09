using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery_System__Team_Enif_.Data;
using Delivery_System__Team_Enif_.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Delivery_System__Team_Enif_.Models;
using System.Security.Claims;

namespace Delivery_System__Team_Enif_.Controllers
{
    public class PackageController : Controller
    {
        private readonly ProjectDbContext _projectDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public PackageController(ProjectDbContext projectDbContext, UserManager<ApplicationUser> userManager)
        {
            this._projectDbContext = projectDbContext;
            this._userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ApplicationUser currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                RedirectToAction("Login", "Account");
            }

            bool isUserRolePermit = await IsUserRolesPermitAsync(currentUser);
            if (!isUserRolePermit)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            List<Package> packages;

            if (User.IsInRole("User"))
            {
                packages = _projectDbContext.Packages
                    .Include(p => p.CreatedBy)
                    .Include(p => p.Office)
                    .Include(p => p.DeliveryOption)
                    .Include(p => p.DeliveryType)
                    .Include(p => p.DeliveryStatus)
                    .Where(p => p.CreatedBy.Id == currentUser.Id)
                    .OrderBy(p => p.CreatedDate)
                    .ThenBy(p => p.DeliveryTypeId == (int)DeliveryTypeEnum.Express ? 0 : 1)
                    .ToList();
            }
            else
            {
                if (User.IsInRole("Office assistant") && currentUser.OfficeId == null)
                {
                    ModelState.AddModelError(string.Empty, "No packages found due to no office assigned to current user");
                    return View();
                }

                if (User.IsInRole("Office assistant"))
                {
                    packages = _projectDbContext.Packages
                                        .Include(p => p.CreatedBy)
                                       .Include(p => p.Office)
                                       .Include(p => p.DeliveryOption)
                                       .Include(p => p.DeliveryType)
                                       .Include(p => p.DeliveryStatus)
                                       .Where(p => p.OfficeId == currentUser.OfficeId)
                                       .OrderBy(p => p.CreatedDate)
                                       .ThenBy(p => p.DeliveryTypeId == (int)DeliveryTypeEnum.Express ? 0 : 1)
                                       .ToList();
                }
                else
                {
                    packages = _projectDbContext.Packages
                    .Include(p => p.CreatedBy)
                    .Include(p => p.Office)
                    .Include(p => p.DeliveryOption)
                    .Include(p => p.DeliveryType)
                    .Include(p => p.DeliveryStatus)
                    .OrderBy(p => p.CreatedDate)
                    .ThenBy(p => p.DeliveryTypeId == (int)DeliveryTypeEnum.Express ? 0 : 1)
                    .ToList();
                }
            }

            PackageViewModel viewModel = new PackageViewModel
            {
                Packages = packages,
                CurrentUserId = currentUser.Id
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ApplicationUser currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                RedirectToAction("Login", "Account");
            }

            bool isUserRolePermit = await IsUserRolesPermitAsync(currentUser);
            if (!isUserRolePermit)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            IQueryable<Office> offices = _projectDbContext.Offices.Include(o => o.Employees);
            int officeIdAssignedToUser = -1;
            if (User.IsInRole("Office assistant"))
            {
                var officeAssignedToUser = await _projectDbContext.Offices
                                            .Where(o => o.Employees.Any(e => e.Id == currentUser.Id))
                                            .FirstOrDefaultAsync();

                if (officeAssignedToUser != null)
                {
                    offices = offices.Where(o => o.Id == officeAssignedToUser.Id);
                    officeIdAssignedToUser = officeAssignedToUser.Id;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No office assigned to current Office Assistance");
                    return RedirectToAction("Index");
                }
            }
            
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
                DeliveryDate = DateTime.Now,
                AvailableOffices = await offices.ToListAsync()
            };

            if (User.IsInRole("User"))
            {
                model.SenderName = currentUser.Name;
                model.SenderAddress = currentUser.Address;
            }

            if (User.IsInRole("Office assistant") && officeIdAssignedToUser != -1)
            {
                model.OfficeId = officeIdAssignedToUser;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConfirm(Package package)
        {
            ApplicationUser currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                RedirectToAction("Login", "Account");
            }

            bool isUserRolePermit = await IsUserRolesPermitAsync(currentUser);
            if (!isUserRolePermit)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (User.IsInRole("Office assistant"))
            {
                package.OfficeId = currentUser.OfficeId;
            }

            package.CreatedBy = currentUser;
            package.CreatedDate = DateTime.Now;
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

            ApplicationUser currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                RedirectToAction("Login", "Account");
            }

            bool isUserRolePermit = await IsUserRolesPermitAsync(currentUser);
            if (!isUserRolePermit)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            IQueryable<Package> packageQuery = _projectDbContext.Packages
                                                .Include(p => p.CreatedBy)
                                                .Include(p => p.Office);

            if (User.IsInRole("Office assistant") && currentUser.OfficeId != null)
            {
                packageQuery = packageQuery.Where(p => p.OfficeId == currentUser.OfficeId);
            }

            var package = await packageQuery.FirstOrDefaultAsync(p => p.Id == id);
            if (package == null)
            {
                ModelState.AddModelError(string.Empty, "Not found a package with provided package id");
                return RedirectToAction("Index");
            }

            if (User.IsInRole("User") && package.CreatedBy.Id != currentUser.Id)
            {
                ModelState.AddModelError(string.Empty, "No package with provided package id or package not created by you");
                return RedirectToAction("Index");
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
                DeliveryDate = package.DeliveryDate,

                CreatedDate = package.CreatedDate,
                CreatedByUserId = package.CreatedBy.Id,
                CreatedByUser = package.CreatedBy.Name
            };

            if (package.Office != null)
            {
                viewModel.OfficeSelected = package.Office.Name;
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound("The provided package id is null");
            }

            ApplicationUser currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                RedirectToAction("Login", "Account");
            }

            bool isUserRolePermit = await IsUserRolesPermitAsync(currentUser);
            if (!isUserRolePermit)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            IQueryable<Package> packageQuery = _projectDbContext.Packages   
                                                .Include(p => p.CreatedBy)
                                                .Include(p => p.Office);

            if (User.IsInRole("Office assistant") && currentUser.OfficeId != null)
            {
                packageQuery = packageQuery.Where(p => p.OfficeId == currentUser.OfficeId);
            }

            var package = await packageQuery.FirstOrDefaultAsync(p => p.Id == id);
            if (package == null)
            {
                ModelState.AddModelError(string.Empty, "Not found a package with provided package id");
                return RedirectToAction("Index");
            }

            if (User.IsInRole("User") && package.CreatedBy.Id != currentUser.Id)
            {
                ModelState.AddModelError(string.Empty, "No package with provided package id or package not created by you");
                return RedirectToAction("Index");
            }

            if (package.DeliveryStatusId != (int)DeliveryStatusEnum.Pending)
            {
                ModelState.AddModelError(string.Empty, "The package is not editable!");
                return RedirectToAction("Details");
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
                DeliveryDate = package.DeliveryDate,

                CreatedDate = package.CreatedDate,
                CreatedByUserId = package.CreatedBy.Id,
                CreatedByUser = package.CreatedBy.Name
            };

            var officeQuery = _projectDbContext.Offices.AsQueryable();
            if (User.IsInRole("Office assistant"))
            {
                var officeId = currentUser.OfficeId;
                if (officeId == null)
                {
                    ModelState.AddModelError(string.Empty, "No office assigned to the current Office Assistant");
                    return View(viewModel);
                }
                
                var availableUserOffices = await officeQuery.Where(o => o.Id == officeId).ToListAsync();
                if (!availableUserOffices.Any())
                {
                    ModelState.AddModelError(string.Empty, "No office assigned to the current Office Assistant");
                    return View(viewModel);
                }
                else
                {
                    viewModel.OfficeId = officeId.Value;
                    viewModel.AvailableOffices = availableUserOffices;
                }
            } else {
                if (package.OfficeId != null)
                {
                    viewModel.OfficeId = package.OfficeId.Value;
                }
                viewModel.AvailableOffices = await officeQuery.ToListAsync();
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PackageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            ApplicationUser currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                RedirectToAction("Login", "Account");
            }

            bool isUserRolePermit = await IsUserRolesPermitAsync(currentUser);
            if (!isUserRolePermit)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var package = await _projectDbContext.Packages
                                .Include(p => p.CreatedBy)
                                .Include(p => p.Office)
                                .FirstOrDefaultAsync(m => m.Id == viewModel.Id);

            if (package == null)
            {
                ModelState.AddModelError(string.Empty, "No package with provided package id found");
                return RedirectToAction("Index");
            }

            if (User.IsInRole("User") && package.CreatedBy.Id != currentUser.Id)
            {
                ModelState.AddModelError(string.Empty, "No package with provided package id or package not created by you");
                return RedirectToAction("Index");
            }

            package.SenderName = viewModel.SenderName;
            package.RecipientName = viewModel.RecipientName;
            package.SenderAddress = viewModel.SenderAddress;
            package.RecipientAddress = viewModel.RecipientAddress;
            package.Length = viewModel.Length;
            package.Width = viewModel.Width;
            package.Hight = viewModel.Hight;
            package.Weight = viewModel.Weight;
            package.OfficeId = viewModel.OfficeId;

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

            ApplicationUser currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                RedirectToAction("Login", "Account");
            }

            bool isUserRolePermit = await IsUserRolesPermitAsync(currentUser);
            if (!isUserRolePermit)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            IQueryable<Package> packageQuery = _projectDbContext.Packages
                                                .Include(p => p.CreatedBy)
                                                .Include(p => p.Office);

            if (User.IsInRole("Office assistant") && currentUser.OfficeId != null)
            {
                packageQuery = packageQuery.Where(p => p.OfficeId == currentUser.OfficeId);
            }

            var package = await packageQuery.FirstOrDefaultAsync(p => p.Id == id);
            if (package == null)
            {
                ModelState.AddModelError(string.Empty, "Not found a package with provided package id");
                return RedirectToAction("Index");
            }

            if (User.IsInRole("User") && package.CreatedBy.Id != currentUser.Id)
            {
                ModelState.AddModelError(string.Empty, "No package with provided package id or package not created by you");
                return RedirectToAction("Index");
            }

            if (package.DeliveryStatusId != (int)DeliveryStatusEnum.Pending)
            {
                ModelState.AddModelError(string.Empty, "The package can not be deleted!");
                return RedirectToAction("Index");
            }

            PackageViewModel viewModel = new PackageViewModel
            {
                Id = package.Id,
                SenderName = package.SenderName,
                RecipientName = package.RecipientName,
                DeliveryOptionSelected = (DeliveryOptionEnum)Enum.ToObject(typeof(DeliveryOptionEnum), package.DeliveryOptionId),
                DeliveryTypeSelected = (DeliveryTypeEnum)Enum.ToObject(typeof(DeliveryTypeEnum), package.DeliveryTypeId),
                DeliveryStatusSelected = (DeliveryStatusEnum)Enum.ToObject(typeof(DeliveryStatusEnum), package.DeliveryStatusId),
                DeliveryDate = package.DeliveryDate,

                CreatedDate = package.CreatedDate,
                CreatedByUserId = package.CreatedBy.Id,
                CreatedByUser = package.CreatedBy.Name
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ApplicationUser currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                RedirectToAction("Login", "Account");
            }

            bool isUserRolePermit = await IsUserRolesPermitAsync(currentUser);
            if (!isUserRolePermit)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            IQueryable<Package> packageQuery = _projectDbContext.Packages
                                                .Include(p => p.CreatedBy)
                                                .Include(p => p.Office);

            if (User.IsInRole("Office assistant") && currentUser.OfficeId != null)
            {
                packageQuery = packageQuery.Where(p => p.OfficeId == currentUser.OfficeId);
            }

            var package = await packageQuery.FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
            {
                ModelState.AddModelError(string.Empty, "No package with provided package id found");
                return RedirectToAction("Index");
            }

            if (User.IsInRole("User") && package.CreatedBy.Id != currentUser.Id)
            {
                ModelState.AddModelError(string.Empty, "No package with provided package id or package not created by you");
                return RedirectToAction("Index");
            }

            if (package.DeliveryStatusId != (int)DeliveryStatusEnum.Pending)
            {
                ModelState.AddModelError(string.Empty, "The package can not be deleted!");
                return RedirectToAction("Index");
            }

            _projectDbContext.Packages.Remove(package);
            await _projectDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
            return _projectDbContext.Packages.Any(e => e.Id == id);
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                return null;
            }

            return currentUser;
        }

        private async Task<bool> IsUserRolesPermitAsync(ApplicationUser applicationUser)
        {
            return User.IsInRole("Admin") || User.IsInRole("Office assistant") || User.IsInRole("User");
        }
    }
}
