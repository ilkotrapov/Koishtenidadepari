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

namespace Delivery_System__Team_Enif_.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly ProjectDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeliveryController(ProjectDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Delivery
        public async Task<IActionResult> Index()
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.DeliveryOption)
                .Include(d => d.DeliveryType)
                .Include(d => d.DeliveryStatus)
                .Include(d => d.Courier)
                .ToListAsync();

            var viewModel = new DeliveryViewModel
            {
                Deliveries = deliveries
            };

            return View(viewModel);
        }



        // GET: Delivery/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // GET: Delivery/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var couriers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Courier"))
                {
                    couriers.Add(user);
                }
            }

            ViewBag.CourierId = new SelectList(couriers, "Id", "Name");

            ViewBag.DeliveryOptions = Enum.GetValues(typeof(DeliveryOptionEnum))
                .Cast<DeliveryOptionEnum>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString().Replace("_", " ")
                }).ToList();

            ViewBag.DeliveryStatuses = Enum.GetValues(typeof(DeliveryStatusEnum))
                .Cast<DeliveryStatusEnum>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString().Replace("_", " ")
                }).ToList();

            ViewBag.DeliveryTypes = Enum.GetValues(typeof(DeliveryTypeEnum))
                .Cast<DeliveryTypeEnum>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString().Replace("_", " ")
                }).ToList();


            return View();
        }


        // POST: Delivery/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryViewModel deliveryViewModel)
        {
            if (ModelState.IsValid)
            {
                var delivery = new Delivery
                {
                    Id = deliveryViewModel.Id,
                    PackageId = deliveryViewModel.PackageId,
                    CourierId = deliveryViewModel.CourierId,
                    PickupTime = deliveryViewModel.PickupTime,
                    DeliveryTime = deliveryViewModel.DeliveryTime,
                    DeliveryOptionId = deliveryViewModel.DeliveryOptionId,
                    DeliveryTypeId = deliveryViewModel.DeliveryTypeId,
                    DeliveryStatusId = deliveryViewModel.DeliveryStatusId
                };

                Console.WriteLine("Submitted DeliveryStatusId: " + deliveryViewModel.DeliveryStatusId);
                _context.Deliveries.Add(delivery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var allUsers = await _userManager.Users.ToListAsync();
            var couriers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Courier"))
                {
                    couriers.Add(user);
                }
            }

            ViewBag.CourierId = new SelectList(couriers, "Id", "Name");

            ViewBag.DeliveryOptions = Enum.GetValues(typeof(DeliveryOptionEnum))
                .Cast<DeliveryOptionEnum>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString().Replace("_", " ")
                }).ToList();

            ViewBag.DeliveryStatuses = Enum.GetValues(typeof(DeliveryStatusEnum))
                .Cast<DeliveryStatusEnum>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString().Replace("_", " ")
                }).ToList();

            ViewBag.DeliveryTypes = Enum.GetValues(typeof(DeliveryTypeEnum))
                .Cast<DeliveryTypeEnum>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString().Replace("_", " ")
                }).ToList();


            return View(deliveryViewModel);
        }




        // GET: Delivery/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
                return NotFound();

            var couriers = await _context.Users.ToListAsync();
            ViewBag.CourierId = new SelectList(couriers, "Id", "Name", delivery.CourierId);

            return View(delivery);
        }

        // POST: Delivery/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PackageId,CourierId,PickupTime,DeliveryTime,DeliveryStatus")] DeliveryViewModel delivery)
        {
            if (id != delivery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var updatedDelivery = new Delivery
                    {
                        Id = delivery.Id,
                        PackageId = delivery.PackageId,
                        CourierId = delivery.CourierId,
                        PickupTime = delivery.PickupTime,
                        DeliveryTime = delivery.DeliveryTime,
                        DeliveryStatus = delivery.DeliveryStatus,
                        // Optional: If you’re using DeliveryOptionId
                        DeliveryOptionId = delivery.DeliveryOptionId
                    };

                    _context.Update(updatedDelivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(delivery.Id))
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

            // 🧠 Rebuild the dropdown if validation fails
            var allUsers = await _userManager.Users.ToListAsync();
            var couriers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Courier"))
                {
                    couriers.Add(user);
                }
            }

            ViewBag.CourierId = new SelectList(couriers, "Id", "Name", delivery.CourierId);

            return View(delivery);
        }


        // GET: Delivery/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // POST: Delivery/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryExists(int id)
        {
            return _context.Deliveries.Any(e => e.Id == id);
        }

    }
}
