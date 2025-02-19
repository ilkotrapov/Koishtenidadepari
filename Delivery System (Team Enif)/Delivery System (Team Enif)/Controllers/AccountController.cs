using Delivery_System__Team_Enif_.Migrations;
using System.Threading.Tasks;
using Delivery_System__Team_Enif_.Models;
using Delivery_System__Team_Enif_.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using Microsoft.EntityFrameworkCore;
using Delivery_System__Team_Enif_.Models.Account;

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

                var user = new ApplicationUser {
                    UserName = model.Email, 
                    Email = model.Email,
                    Name = model.Name,
                    PhoneNumber = model.Phone,
                    Address = model.Address
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
                return BadRequest("A token and user ID must be provided for email confirmation.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewData["Success"] = "Thank you for confirming your email. Wait your submission to be approved by admin.";
            }
            else
            {
                ViewData["Error"] = "There was a problem confirming your email. Please try again";
            }
            return View();
        }

        // GET: Admin/PendingUsers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingUsers()
        {
            var pendingUsers = _userManager.Users.Where(u => u.ApprovalStatus == ApprovalStatus.Pending).ToList();
            return View(pendingUsers);
        }

        // POST: Admin/ApproveOrReject
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveOrReject(string userId, string action)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(action))
            {
                return BadRequest("Invalid data.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (action == "approve")
            {
                user.ApprovalStatus = ApprovalStatus.Approved;
            }
            else if (action == "reject")
            {
                user.ApprovalStatus = ApprovalStatus.Rejected;
            }
            else {
                return BadRequest("Unable to update user status.");
            }

            await _userManager.UpdateAsync(user);

            if (user.ApprovalStatus == ApprovalStatus.Approved)
            {
                user.ApprovalStatus = ApprovalStatus.Approved;
                await _userManager.UpdateAsync(user);

                // Send email notification
                var subject = "Your Account has been Approved";
                var message = "Your account has been approved by the admin. You can login";
                await _emailSender.SendEmailAsync(user.Email, subject, message);                
            }

            return RedirectToAction("PendingUsers");
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
                    if (user.ApprovalStatus == ApprovalStatus.Pending) // Check if the user is pending approval
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
                applicationUser.Address = model.Address;  // Assuming Address is a property in ApplicationUser

                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded)
                {
                    // Optionally, you can send a success message here
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("EditProfile"); // Redirect to the same page or elsewhere
                }
                else
                {
                    // If there are errors, add them to ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

    }
}
