using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Delivery_System__Team_Enif_.Data;
using Delivery_System__Team_Enif_.Models;
using Microsoft.AspNetCore.Identity;
using Delivery_System__Team_Enif_.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Delivery_System__Team_Enif_.Controllers
{
    public class OfficeController : Controller
    {
        private readonly ProjectDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OfficeController(ProjectDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Office
        public async Task<IActionResult> Index()
        {
            var offices = await _context.Offices.Include(o => o.Employees).ToListAsync();
            OfficeViewModel officeViewModel = new OfficeViewModel
            {
                Offices = offices
            };
            return View(officeViewModel);
        }

        // GET: Office/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "The provided office id is null");
                return View();
            }

            var officeEntity = await _context.Offices.FirstOrDefaultAsync(m => m.Id == id);
            if (officeEntity == null)
            {
                ModelState.AddModelError(string.Empty, "No office entity with provided office id");
                return View();
            }

            IEnumerable<ApplicationUserWithRolesViewModel> employees = await GetOfficeEmployees(officeEntity.Id);
            var officeViewModel = new OfficeViewModel
            {
                Id = officeEntity.Id,
                Name = officeEntity.Name,
                Location = officeEntity.Location,
                ContactInfo = officeEntity.ContactInfo,
                WorkingHours = officeEntity.WorkingHours,
                Employees = employees
            };
            return View(officeViewModel);
        }

        // GET: Office/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Office/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Location,ContactInfo,WorkingHours")] OfficeViewModel officeViewModel)
        {
            if (ModelState.IsValid)
            {
                bool officeExists = await _context.Offices
            .AnyAsync(o => o.Name.ToLower() == officeViewModel.Name.ToLower() &&
                           o.Location.ToLower() == officeViewModel.Location.ToLower());

                if (officeExists)
                {
                    ModelState.AddModelError("Name", "An office with the same name and location already exists.");
                    return View(officeViewModel);
                }

                Office officeEntity = new Office
                {
                    Name = officeViewModel.Name,
                    Location = officeViewModel.Location,
                    ContactInfo = officeViewModel.ContactInfo,
                    WorkingHours = officeViewModel.WorkingHours
                };

                _context.Add(officeEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index"); ;
        }

        // GET: Office/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "The provided office id is null");
                return View();
            }

            var officeEntity = await _context.Offices
                                .Include(o => o.Employees)
                                .FirstOrDefaultAsync(o => o.Id == id);

            if (officeEntity == null)
            {
                ModelState.AddModelError(string.Empty, "No office entity with provided office id");
                return View();
            }

            var officeViewModel = new OfficeViewModel
            {
                Id = officeEntity.Id,
                Name = officeEntity.Name,
                Location = officeEntity.Location,
                ContactInfo = officeEntity.ContactInfo,
                WorkingHours = officeEntity.WorkingHours
            };

            return View(officeViewModel);
        }

        // POST: Office/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Location,ContactInfo,WorkingHours")] OfficeViewModel officeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(officeViewModel);
            }

            var officeEntity = await _context.Offices.FindAsync(id);
            if (officeEntity == null)
            {
                ModelState.AddModelError(string.Empty, "No office found with the provided office id");
                return View(officeViewModel);
            }

            bool officeExists = await _context.Offices.AnyAsync(o => o.Name.ToLower() == officeViewModel.Name.ToLower()
                    && o.Location.ToLower() == officeViewModel.Location.ToLower()
                    && o.Id != id);

            if (officeExists)
            {
                ModelState.AddModelError("Name", "An office with the same name and location already exists.");
                return View(officeViewModel);
            }

            officeEntity.Name = officeViewModel.Name;
            officeEntity.Location = officeViewModel.Location;
            officeEntity.ContactInfo = officeViewModel.ContactInfo;
            officeEntity.WorkingHours = officeViewModel.WorkingHours;

            _context.Update(officeEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Office/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "The provided office id is null");
                return View();
            }

            var officeEntity = await _context.Offices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (officeEntity == null)
            {
                ModelState.AddModelError(string.Empty, "No office entity with provided office id");
                return View();
            }

            var officeViewModel = new OfficeViewModel
            {
                Id = officeEntity.Id,
                Name = officeEntity.Name,
                Location = officeEntity.Location,
                ContactInfo = officeEntity.ContactInfo,
                WorkingHours = officeEntity.WorkingHours
            };
            return View(officeViewModel);
        }

        // POST: Office/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var officeEntity = await _context.Offices
                                .Include(o => o.Employees)
                                .FirstOrDefaultAsync(o => o.Id == id);
            if (officeEntity == null)
            {
                ModelState.AddModelError(string.Empty, "No office entity with provided office id");
                return View();
            }

            if (officeEntity.Employees.Any())
            {
                ModelState.AddModelError(string.Empty, "There are employees assigned. Can not delete office details!");
                return View(new OfficeViewModel
                            {
                                Id = officeEntity.Id,
                                Name = officeEntity.Name,
                                Location = officeEntity.Location,
                                ContactInfo = officeEntity.ContactInfo,
                                WorkingHours = officeEntity.WorkingHours
                            }
                        );
            }
            if (officeEntity != null)
            {
                _context.Offices.Remove(officeEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> LoadUsersWithoutOffice(int? officeId, string selectedRole)
        {
            if (officeId == null)
            {
                return PartialView("_UsersWithoutOffice", null);
            }

            var officeEntity = await _context.Offices.FirstOrDefaultAsync(m => m.Id == officeId);
            if (officeEntity == null)
            {
                return PartialView("_UsersWithoutOffice", null);
            }

            var allUsers = await _userManager.Users.Where(u=>u.isActive && u.OfficeId == null).ToListAsync();
            if (!allUsers.Any())
            {
                return PartialView("_UsersWithoutOffice", null);
            }

            var selectedUsersByRoles = allUsers.Where(user => _userManager.IsInRoleAsync(user, "Courier").Result || _userManager.IsInRoleAsync(user, "Office assistant").Result);
            if (!selectedUsersByRoles.Any())
            {
                return PartialView("_UsersWithoutOffice", null);
            }

            var courierRole = await _roleManager.FindByNameAsync("Courier");
            var officeAssistantRole = await _roleManager.FindByNameAsync("Office assistant");
              
            var filteredUsers = new List<ApplicationUser>();
            foreach (var user in selectedUsersByRoles)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (selectedRole == "Courier" && courierRole != null && userRoles.Contains(courierRole.Name))
                {
                    filteredUsers.Add(user);
                }
                
                if (selectedRole == "Office assistant" && officeAssistantRole != null && userRoles.Contains(officeAssistantRole.Name))
                {
                    filteredUsers.Add(user);
                }
            }

            return PartialView("_UsersWithoutOffice", filteredUsers);
        }

        public async Task<IActionResult> UsersWithOfficeAssigned(int? officeId)
        {
            if (officeId == null)
            {
                return PartialView("_UsersWithOfficeAssigned", null);
            }

            var officeEntity = await _context.Offices.FirstOrDefaultAsync(m => m.Id == officeId);
            if (officeEntity == null)
            {
                return PartialView("_UsersWithOfficeAssigned", null);
            }

            var allUsers = await _userManager.Users.ToListAsync();
            if (!allUsers.Any())
            {
                return PartialView("_UsersWithOfficeAssigned", null);
            }

            var selectedUsersByRoles = allUsers.Where(user => _userManager.IsInRoleAsync(user, "Courier").Result || _userManager.IsInRoleAsync(user, "Office assistant").Result);
            if (!selectedUsersByRoles.Any())
            {
                return PartialView("_UsersWithOfficeAssigned", null);
            }

            var courierRole = await _roleManager.FindByNameAsync("Courier");
            var officeAssistantRole = await _roleManager.FindByNameAsync("Office assistant");
            var usersWithOfficeAssigned = selectedUsersByRoles.Where(u => u.OfficeId != null && u.OfficeId == officeId).ToList();

            return PartialView("_UsersWithOfficeAssigned", usersWithOfficeAssigned);
        }

        public async Task<IActionResult> AddEmployees(int? id, string selectedRole = "Courier")
        {
            if (id == null)
            {
                ModelState.AddModelError("officeId", "The provided office id is null");
                return View(new OfficeEmployeesViewModel { SelectedRole = selectedRole });
            }

            var officeEntity = await _context.Offices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (officeEntity == null)
            {
                ModelState.AddModelError(string.Empty, "No office entity with provided office id");
                return View(new OfficeEmployeesViewModel { SelectedRole = selectedRole });
            }

            var usersWithoutOfficeAssigned = await _userManager.Users.Where(u => u.isActive && u.OfficeId == null).ToListAsync();
            var selectedUsersByRoles = usersWithoutOfficeAssigned.Where(user => _userManager.IsInRoleAsync(user, "Courier").Result || _userManager.IsInRoleAsync(user, "Office assistant").Result);

            if (!selectedUsersByRoles.Any())
            {
                ModelState.AddModelError(string.Empty, "No users available to add as employees.");
                return View(new OfficeEmployeesViewModel
                {
                    SelectedOfficeId = officeEntity.Id,
                    SelectedRole = selectedRole
                });
            }

            var courierRole = await _roleManager.FindByNameAsync("Courier");
            var officeAssistantRole = await _roleManager.FindByNameAsync("Office assistant");
            var filteredUsers = new List<ApplicationUser>();
            foreach (var user in usersWithoutOfficeAssigned)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (selectedRole == "Courier" && courierRole != null && userRoles.Contains(courierRole.Name))
                {
                    filteredUsers.Add(user);
                }
                else if (selectedRole == "Office assistant" && officeAssistantRole != null && userRoles.Contains(officeAssistantRole.Name))
                {
                    filteredUsers.Add(user);
                }
            }

            var model = new OfficeEmployeesViewModel
            {
                SelectedOfficeId = officeEntity.Id,
                SelectedRole = selectedRole,
                Users = filteredUsers
            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployees(int officeId, string[] selectedUserId)
        {
            var officeEntity = await _context.Offices.FindAsync(officeId);
            if (officeEntity == null)
            {
                ModelState.AddModelError(string.Empty, "No office entity with provided office id");
                return View();
            }

            if (selectedUserId == null || selectedUserId.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select at least one user to assign.");
                return RedirectToAction("AddEmployees", new {officeId });
            }

            var userIds = selectedUserId.ToList();
            var usersToAssign = _context.Users.Where(u => userIds.Contains(u.Id) && u.OfficeId == null).ToList();
            if (!usersToAssign.Any())
            {
                ModelState.AddModelError(string.Empty, "The selected users are either already assigned to an office or don't exist.");
                return View();
            }

            foreach (var user in usersToAssign)
            {
                user.OfficeId = officeId;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = officeId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveEmployee(int officeId, string selectedUserId)
        {
            var officeEntity = await _context.Offices.FindAsync(officeId);
            if (officeEntity == null)
            {
                ModelState.AddModelError(string.Empty, "No office entity with provided office id");
                return View();
            }

            if (selectedUserId == null)
            {
                ModelState.AddModelError(string.Empty, "Please select an user to remove as employee.");
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == selectedUserId && u.OfficeId == officeId);
            if (user == null)
            { 
                ModelState.AddModelError(string.Empty, "The selected user is not employee in the office");
                return View();
            }

            user.OfficeId = null;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = officeId });
        }

        private async Task<IEnumerable<ApplicationUserWithRolesViewModel>> GetOfficeEmployees(int id)
        {
            var allUsersByOffice = await _userManager.Users
                                    .Where(u => u.OfficeId == id)
                                    .ToListAsync(); 

            var employees = new List<ApplicationUserWithRolesViewModel>();

            if (allUsersByOffice.Any())
            {
                foreach (var user in allUsersByOffice)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var employee = new ApplicationUserWithRolesViewModel
                    {
                        User = user,
                        Roles = roles.ToList()
                    };

                    if (employee.Roles.Contains("Courier") || employee.Roles.Contains("Office assistant"))
                    {
                        employees.Add(employee);
                    }
                }
            }

            return employees;
        }
    }
}
