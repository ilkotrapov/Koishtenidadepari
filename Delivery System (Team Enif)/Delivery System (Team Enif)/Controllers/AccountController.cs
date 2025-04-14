using Delivery_System__Team_Enif_.Models;
using Delivery_System__Team_Enif_.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Delivery_System__Team_Enif_.Models.Account;
using Delivery_System__Team_Enif_.Data;
using Microsoft.EntityFrameworkCore;

namespace Delivery_System__Team_Enif_.Controllers
{ 
    public class AccountController : Controller
    {
        private readonly ProjectDbContext _projectDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ProjectDbContext projectDbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _projectDbContext = projectDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        // Register action
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Register POST action
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    // Add an error to the model state if the user already exists
                    ModelState.AddModelError(string.Empty, "A user with this email already exists.");
                    return View(model);
                }

                var user = new ApplicationUser {
                    UserName = model.Email, 
                    Email = model.Email,
                    Name = model.Name,
                    PhoneNumber = model.Phone,
                    Address = model.Address,
                    isActive = false
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var role = model.Role;
                    var roleExist = await _roleManager.RoleExistsAsync(model.Role);
                    if (roleExist)
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Role does not exist.");
                        return View(model);
                    }
                    
                    // Generate the email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, protocol: Request.Scheme);

                        // Send confirmation email
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");

                    _logger.LogInformation("User registered successfully.");

                    return RedirectToAction("RegistrationConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // RegistrationConfirmation view
        [HttpGet]
        public IActionResult RegistrationConfirmation()
        {
            return View();
        }

        // Confirm Email
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
             if (userId == null || token == null)
            {
                ViewData["Error"] = "There is a problem with your account.";
                _logger.LogWarning("A token and user ID must be provided for email confirmation.");
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewData["Error"] = "There is a problem with your account.";
                _logger.LogWarning($"Unable to load user with ID '{userId}'.");
                return View();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.isActive = true;
                var newResult = await _userManager.UpdateAsync(user);
                if (newResult.Succeeded)
                {
                    ViewData["Success"] = "Thank you for confirming your email. You can login.";
                    _logger.LogInformation($"User with ID '{userId}' was registered and activated successfully.");
                }
                else
                {
                    ViewData["Error"] = "There is a problem with your account activation.";
                    _logger.LogWarning($"Unable to activate user with ID '{userId}'.");
                }
            
            }
            else
            {
                ViewData["Error"] = "There was a problem confirming your email. Please try again";
            }
            return View();
        }

        // Login action
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login POST action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!user.isActive) 
                    {
                        ModelState.AddModelError(string.Empty, "There is a problem with your account.");
                        return View(model);
                    }

                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home"); // Redirect after successful login
                    }

                    if (result.IsLockedOut)
                    {
                        return View("Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }

            }
            return View(model);
        }

        // Logout action
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var applicationUser = user as ApplicationUser;
            if (applicationUser != null)
            {
                var model = new ProfileViewModel
                {
                    Name = applicationUser.Name,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Address = user.Address
                };
                return View(model);
            } else
            {
                ModelState.AddModelError(string.Empty, "The user is not of the expected type.");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var emailExist = await _userManager.FindByEmailAsync(model.Email);
                if (emailExist != null && emailExist.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "Email is already in use by another account.");
                }

                var applicationUser = user as ApplicationUser;
                applicationUser.Name = model.Name;
                applicationUser.UserName = model.Email;
                applicationUser.Email = model.Email;
                applicationUser.PhoneNumber = model.Phone;
                applicationUser.Address = model.Address;

                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("EditProfile"); // Redirect to the same page or elsewhere
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            var nonAdminUsers = await (from user in _projectDbContext.Users
                                       join userRole in _projectDbContext.UserRoles on user.Id equals userRole.UserId
                                       join role in _projectDbContext.Roles on userRole.RoleId equals role.Id
                                       where role.Name != "Admin"
                                       select user)
                            .ToListAsync();
            return View(nonAdminUsers);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateOrDeactivateUser(string userId, string action)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "Invalid data.");
                return RedirectToAction("Users");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return RedirectToAction("Users");
            }

            if (action == "1")
            {
                user.isActive = true;
            }
            else
            { 
                user.isActive = false;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded && action == "0")
            {
                var message = "Your account has been de-activated";
                await _emailSender.SendEmailAsync(user.Email, message, message);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<ApplicationUserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // ✅ This gets role names like "Admin", "Courier"

                userViewModels.Add(new ApplicationUserWithRolesViewModel
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels); // 👈 View must be strongly typed: @model IEnumerable<ApplicationUserWithRolesViewModel>
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
