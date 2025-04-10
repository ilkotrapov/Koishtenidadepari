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
                return NotFound();

            var delivery = await _context.Deliveries
                .Include(d => d.DeliveryOption)
                .Include(d => d.DeliveryType)
                .Include(d => d.DeliveryStatus)
                .Include(d => d.Courier)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (delivery == null)
                return NotFound();

            var viewModel = new DeliveryViewModel
            {
                Id = delivery.Id,
                PackageId = delivery.PackageId,
                CourierId = delivery.CourierId,
                Courier = delivery.Courier,
                CourierName = delivery.Courier?.Name,
                PickupTime = delivery.PickupTime,
                DeliveryTime = delivery.DeliveryTime,
                DeliveryStatus = delivery.DeliveryStatus,
                DeliveryStatusId = delivery.DeliveryStatusId,
                DeliveryOption = delivery.DeliveryOption,
                DeliveryOptionId = delivery.DeliveryOptionId,
                DeliveryType = delivery.DeliveryType,
                DeliveryTypeId = delivery.DeliveryTypeId
            };

            return View(viewModel);
        }

        // GET: Delivery/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new DeliveryViewModel();
            await PopulateDropdowns(model);
            return View(model);
        }

        // POST: Delivery/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryViewModel deliveryViewModel)
        {
            Console.WriteLine("🚀 POST: Create hit");

            if (ModelState.IsValid)
            {
                var delivery = new Delivery
                {
                    PackageId = deliveryViewModel.PackageId,
                    CourierId = deliveryViewModel.CourierId,
                    PickupTime = deliveryViewModel.PickupTime,
                    DeliveryTime = deliveryViewModel.DeliveryTime,
                    DeliveryStatusId = deliveryViewModel.DeliveryStatusId,
                    DeliveryOptionId = deliveryViewModel.DeliveryOptionId,
                    DeliveryTypeId = deliveryViewModel.DeliveryTypeId
                };

                _context.Deliveries.Add(delivery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(deliveryViewModel);
            return View(deliveryViewModel);
        }

        // GET: Delivery/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var delivery = await _context.Deliveries
                .Include(d => d.Courier)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (delivery == null)
                return NotFound();

            var viewModel = new DeliveryViewModel
            {
                Id = delivery.Id,
                PackageId = delivery.PackageId,
                CourierId = delivery.CourierId,
                CourierName = delivery.Courier?.Name,
                PickupTime = delivery.PickupTime,
                DeliveryTime = delivery.DeliveryTime,
                DeliveryStatusId = delivery.DeliveryStatusId,
                DeliveryOptionId = delivery.DeliveryOptionId,
                DeliveryTypeId = delivery.DeliveryTypeId
            };

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Delivery/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeliveryViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var delivery = await _context.Deliveries.FindAsync(id);
                    if (delivery == null)
                        return NotFound();

                    delivery.PackageId = viewModel.PackageId;
                    delivery.CourierId = viewModel.CourierId;
                    delivery.PickupTime = viewModel.PickupTime;
                    delivery.DeliveryTime = viewModel.DeliveryTime;
                    delivery.DeliveryStatusId = viewModel.DeliveryStatusId;
                    delivery.DeliveryOptionId = viewModel.DeliveryOptionId;
                    delivery.DeliveryTypeId = viewModel.DeliveryTypeId;

                    _context.Update(delivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(viewModel.Id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // GET: Delivery/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var delivery = await _context.Deliveries
                .Include(d => d.DeliveryOption)
                .Include(d => d.DeliveryType)
                .Include(d => d.DeliveryStatus)
                .Include(d => d.Courier)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (delivery == null)
                return NotFound();

            var viewModel = new DeliveryViewModel
            {
                Id = delivery.Id,
                PackageId = delivery.PackageId,
                PickupTime = delivery.PickupTime,
                DeliveryTime = delivery.DeliveryTime,
                DeliveryStatus = delivery.DeliveryStatus,
                DeliveryStatusId = delivery.DeliveryStatusId,
                DeliveryOption = delivery.DeliveryOption,
                DeliveryOptionId = delivery.DeliveryOptionId,
                DeliveryType = delivery.DeliveryType,
                DeliveryTypeId = delivery.DeliveryTypeId,
                CourierId = delivery.CourierId,
                CourierName = delivery.Courier?.Name
            };

            return View(viewModel);
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
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryExists(int id)
        {
            return _context.Deliveries.Any(e => e.Id == id);
        }

        private async Task PopulateDropdowns(DeliveryViewModel model)
        {
            var couriers = await _userManager.Users.ToListAsync();
            var courierList = new List<ApplicationUser>();

            foreach (var user in couriers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Courier"))
                    courierList.Add(user);
            }

            ViewBag.CourierId = new SelectList(courierList, "Id", "Name", model.CourierId);

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
        }
    }
}
