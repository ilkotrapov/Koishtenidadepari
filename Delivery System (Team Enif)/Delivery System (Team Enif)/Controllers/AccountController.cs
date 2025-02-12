using Delivery_System__Team_Enif_.Models;
using Delivery_System__Team_Enif_.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Delivery_System__Team_Enif_.Controllers
{ 
    
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
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
        //[ValidateAntiForgeryToken]
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

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
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
                return BadRequest("A token and user ID must be provided for email confirmation.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? "Thank you for confirming your email. You can now log in." : "There was a problem confirming your email. Please try again");
        }

        public async Task<IActionResult> PendingUsers()
        {
            // Retrieve all users who are pending approval
            var users = _userManager.Users.Where(u => u.PendingApproval).ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Set the user as approved
                user.PendingApproval = false;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("PendingUsers");
                }
            }

            return BadRequest("Unable to approve user.");
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
                    if (user.PendingApproval) // Check if the user is pending approval
                    {
                        ModelState.AddModelError(string.Empty, "Your account is pending approval by an administrator.");
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
            }
            return View(model);
        }

        // Logout action
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
