using Delivery_System__Team_Enif_.Models.Stripe;
using Delivery_System__Team_Enif_.Data.Entities;
using Delivery_System__Team_Enif_.Data;
using Delivery_System__Team_Enif_.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Delivery_System__Team_Enif_.Models; // Import StripeSettings

[ApiController]
[Route("api/[controller]")]
public class PaymentController : Controller
{
    private readonly ProjectDbContext _projectDbContext;
    private readonly StripeSettings _stripeSettings;

    public PaymentController(ProjectDbContext projectDbContext, IConfiguration configuration)
    {
        _projectDbContext = projectDbContext ?? throw new ArgumentNullException(nameof(projectDbContext));

        _stripeSettings = new StripeSettings
        {
            SecretKey = configuration["Stripe:SecretKey"],
            PublishableKey = configuration["Stripe:PublishableKey"],
            WebhookSecret = configuration["Stripe:WebhookSecret"]
        };

        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    [HttpGet("process-payment")]
    public IActionResult ProcessPayment(int packageId, long amount)
    {
        // Fetch the package to get customer details (e.g., email)
        var package = _projectDbContext.Packages
            .FirstOrDefault(p => p.Id == packageId);

        if (package == null) return NotFound();

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            PaymentMethodTypes = new List<string> { "card" },
            SuccessUrl = $"https://localhost:7024/api/payment/payment-success?packageId={packageId}",
            CancelUrl = $"https://localhost:7024/api/payment/payment-cancelled?packageId={packageId}",
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "eur",
                        UnitAmount = amount,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Package Delivery"
                        }
                    },
                    Quantity = 1

                }
            },
            Metadata = new Dictionary<string, string> 
            {
                {"packageId", packageId.ToString() }
            }
        };

        var service = new SessionService();
        var session = service.Create(options);

        return Redirect(session.Url); // Redirect to Stripe checkout
    }

    [HttpGet("payment-success")]
    public IActionResult Success(int packageId)
    {
        var package = _projectDbContext.Packages
            .Include(p => p.DeliveryStatus)
            .FirstOrDefault(p => p.Id == packageId);

        if (package == null) return NotFound();

        // Generate tracking number if not exists
        if (string.IsNullOrEmpty(package.TrackingNumber))
        {
            package.TrackingNumber = GenerateTrackingNumber();
            _projectDbContext.SaveChanges();
        }

        return View("~/Views/Delivery/PaymentSuccess.cshtml", package);
    }

    [HttpGet("payment-cancelled")]
    public IActionResult Cancel(int packageId)
    {
        // Eager load status and include tracking
        var package = _projectDbContext.Packages
            .Include(p => p.DeliveryStatus)
            .FirstOrDefault(p => p.Id == packageId);

        if (package != null)
        {
            // Get status ID safely
            var failedStatus = _projectDbContext.DeliveryStatuses
                .FirstOrDefault(s => s.Name == DeliveryStatusEnum.PaymentFailed.ToString());

            if (failedStatus != null)
            {
                package.DeliveryStatusId = failedStatus.Id;
                try
                {
                    _projectDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Log error but still show cancellation page
                    Console.WriteLine($"Error updating status: {ex.Message}");
                }
            }
        }

        return View("~/Views/Delivery/PaymentCancel.cshtml", package);
    }

    [HttpGet("check-statuses")]
    public IActionResult CheckStatuses()
    {
        var statuses = _projectDbContext.DeliveryStatuses.ToList();
        return Json(statuses); // Returns JSON directly
    }

    private string GenerateTrackingNumber()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
    }

    public class CheckoutRequest
    {
        public long Amount { get; set; }
        public string Email { get; set; }
    }


    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _stripeSettings.WebhookSecret // webhook secret from configuration
            );

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                if (session?.Metadata != null && session.Metadata.ContainsKey("packageId"))
                {
                    if (int.TryParse(session.Metadata["packageId"], out int packageId))
                    {
                        var package = await _projectDbContext.Packages.FindAsync(packageId);
                        if (package != null)
                        {
                            package.DeliveryStatusId = (int)DeliveryStatusEnum.Active;
                            await _projectDbContext.SaveChangesAsync();
                        }
                    }
                }
            }

            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine($"⚠️ Webhook error: {e.Message}");
            return BadRequest();
        }
    }

    [HttpPost("send-invoice")]
    public async Task<IActionResult> SendInvoice([FromBody] InvoiceRequest request)
    {
        try
        {
            if (request.Amount <= 0)
            {
                throw new ArgumentException("Invoice amount must be greater than zero.");
            }

            var customerService = new CustomerService();

            // Retrieve or create the customer
            var customers = await customerService.ListAsync(new CustomerListOptions { Email = request.Email });
            var customer = customers.Data.FirstOrDefault() ?? await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = request.Email,
                Description = "Box Delivery Invoice"
            });

            // Create the invoice item with the correct amount
            var invoiceItemService = new InvoiceItemService();
            await invoiceItemService.CreateAsync(new InvoiceItemCreateOptions
            {
                Customer = customer.Id,
                Amount = request.Amount, // Amount in cents
                Currency = "eur",
                Description = $"Box Delivery Service: {request.Amount / 100.0} EUR"
            });

            // Create and send the invoice
            var invoiceService = new InvoiceService();
            var invoice = await invoiceService.CreateAsync(new InvoiceCreateOptions
            {
                Customer = customer.Id,
                CollectionMethod = "send_invoice", // Ensures email invoice is sent
                AutoAdvance = true, // Finalizes the invoice
                DaysUntilDue = 7
            });

            // Finalize and trigger the email notification
            var finalizedInvoice = await invoiceService.FinalizeInvoiceAsync(invoice.Id);

            return Ok(new { success = true, invoiceId = finalizedInvoice.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    public class InvoiceRequest
    {
        public string Email { get; set; }
        public long Amount { get; set; }
    }

    /*[HttpGet("Success")]
    public IActionResult Success(int packageId)
    {
        var package = _projectDbContext.Packages
            .FirstOrDefault(p => p.Id == packageId);

        if (package == null) return NotFound();

        return View(package); // Pass package to view
    }
    */


}