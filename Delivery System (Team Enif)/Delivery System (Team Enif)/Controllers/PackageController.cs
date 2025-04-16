using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Delivery_System__Team_Enif_.Data;
using Delivery_System__Team_Enif_.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Delivery_System__Team_Enif_.Models;
using System.Security.Claims;
using Delivery_System__Team_Enif_.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


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
                packages = _projectDbContext.Packages
                    .Include(p => p.DeliveryOption)
                    .Include(p => p.DeliveryType)
                    .Include(p => p.DeliveryStatus)
                    .OrderBy(p => p.CreatedDate)
                    .ThenBy(p => p.DeliveryTypeId == (int)DeliveryTypeEnum.Express ? 0 : 1)
                    .ToList();
            }
            PackageViewModel viewModel = new PackageViewModel
            {
                Packages = packages
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


            if (User.IsInRole("User"))
            {
                model.SenderName = currentUser.Name;
                model.SenderAddress = currentUser.Address;
            }
            return View(model);
        }

        /*[HttpPost]
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

            package.CreatedBy = currentUser;
            package.CreatedDate = DateTime.Now;
            package.DeliveryStatusId = (int)DeliveryStatusEnum.Pending;
            _projectDbContext.Add(package);
            _projectDbContext.SaveChanges();

            return RedirectToAction("Index");
        }
        */

        [HttpPost]
        public async Task<IActionResult> CreateConfirm(PackageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            ApplicationUser currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            bool isUserRolePermit = await IsUserRolesPermitAsync(currentUser);
            if (!isUserRolePermit)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            decimal packageSize = model.Length * model.Width * model.Hight;

            decimal basePrice = 10;
            decimal weightFee = model.Weight * 5;
            decimal volumeFee = (model.PackageSize) / 2000;
            decimal totalPrice = basePrice + weightFee + volumeFee;
            long amountInCents = (long)(totalPrice * 100);

            var deliveryOption = await _projectDbContext.DeliveryOptions.FindAsync(model.DeliveryOptionId);
            var deliveryType = await _projectDbContext.DeliveryTypes.FindAsync(model.DeliveryTypeId);
            var deliveryStatus = await _projectDbContext.DeliveryStatuses.FindAsync((int)DeliveryStatusEnum.Pending);

            if (deliveryOption == null || deliveryType == null || deliveryStatus == null)
            {
                return BadRequest("Invalid delivery option, type, or status.");
            }

            // Initialize the Package with required CreatedBy and CreatedDate
            var package = new Package
            {
                SenderName = model.SenderName,
                RecipientName = model.RecipientName,
                SenderAddress = model.SenderAddress,
                RecipientAddress = model.RecipientAddress,
                Length = model.Length,
                Width = model.Width,
                Hight = model.Hight,
                Weight = model.Weight,
                DeliveryOptionId = model.DeliveryOptionId,
                DeliveryOption = deliveryOption,
                DeliveryTypeId = model.DeliveryTypeId,
                DeliveryType = deliveryType,
                DeliveryStatusId = (int)DeliveryStatusEnum.Pending,
                DeliveryStatus = deliveryStatus,
                DeliveryDate = model.DeliveryDate,
                CreatedBy = currentUser, // Set required CreatedBy
                CreatedDate = DateTime.Now, // Set CreatedDate
                CurrentLatitude = 40.7128m, // NYC default
                CurrentLongitude = -74.0060m
            };

            _projectDbContext.Packages.Add(package);
            await _projectDbContext.SaveChangesAsync();

            return Redirect($"/api/payment/process-payment?packageId={package.Id}&amount={amountInCents}");
        }


        [HttpGet("TrackPackage")]
        public IActionResult TrackPackage()
        {
            return View(new TrackingViewModel
            {
                CurrentLatitude = 40.7128m,
                CurrentLongitude = -74.0060m
            });
        }

        [HttpPost("TrackPackage")]
        public async Task<IActionResult> TrackPackage(string trackingNumber)
        {
            var vm = new TrackingViewModel();

            try
            {
                var package = await _projectDbContext.Packages
                    .Include(p => p.DeliveryStatus)
                    .Include(p => p.LocationHistory)
                    .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);

                if (package == null)
                {
                    vm.ErrorMessage = "Tracking number not found";
                    return View(vm);
                }

                // Map to ViewModel
                vm.TrackingNumber = package.TrackingNumber;
                vm.CurrentLatitude = package.CurrentLatitude;
                vm.CurrentLongitude = package.CurrentLongitude;
                vm.Status = package.DeliveryStatus?.Name; // Get the name here

                // Map location history
                vm.LocationHistory = package.LocationHistory?.Select(h => new TrackingViewModel.LocationHistoryItem
                {
                    Latitude = h.Latitude,
                    Longitude = h.Longitude
                }).ToList();

                return View(vm);
            }
            catch (Exception ex)
            {
                vm.ErrorMessage = "Error retrieving tracking information";
                Console.WriteLine($"Tracking error: {ex}");
                return View(vm);
            }
        }

        public class UpdateLocationModel
        {
            public string TrackingNumber { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
        }

        [HttpPost("UpdateLocation")]
        [Authorize(Roles = "Admin,Courier,User")]
        public async Task<IActionResult> UpdateLocation(
            [FromBody] UpdateLocationModel model,
            [FromServices] IHubContext<PackageHub> hubContext)
        {
            var package = await _projectDbContext.Packages
                .Include(p => p.LocationHistory)
                .FirstOrDefaultAsync(p => p.TrackingNumber == model.TrackingNumber);

            if (package == null) return NotFound();

            package.CurrentLatitude = model.Latitude;
            package.CurrentLongitude = model.Longitude;

            package.LocationHistory.Add(new PackageLocation
            {
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Timestamp = DateTime.UtcNow
            });

            await _projectDbContext.SaveChangesAsync();

            await hubContext.Clients.Group(model.TrackingNumber)
                .SendAsync("LocationUpdated", model.Latitude, model.Longitude);

            return Ok();
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

            var package = await _projectDbContext.Package
                                .Include(p => p.CreatedBy)
                                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                ModelState.AddModelError(string.Empty, "Not found a package with provided package id");
                return RedirectToAction("Index");
            }

            bool isUser = await _userManager.IsInRoleAsync(currentUser, "User");
            bool isOfficeAssistant = await _userManager.IsInRoleAsync(currentUser, "Office Assistant");

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

            var package = await _projectDbContext.Package
                                .Include(p => p.CreatedBy)
                                .FirstOrDefaultAsync(m => m.Id == id);
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

            var package = await _projectDbContext.Package
                                .Include(p => p.CreatedBy)
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

            var package = await _projectDbContext.Package
                                .Include(p => p.CreatedBy)
                                .FirstOrDefaultAsync(m => m.Id == id);
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

            var package = await _projectDbContext.Package
                                .Include(p => p.CreatedBy)
                                .FirstOrDefaultAsync(m => m.Id == id);

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
