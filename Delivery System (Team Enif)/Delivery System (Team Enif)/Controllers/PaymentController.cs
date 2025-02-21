using Delivery_System__Team_Enif_.Models.Stripe;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Delivery_System__Team_Enif_.Models; // Import StripeSettings

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly StripeSettings _stripeSettings;

    public PaymentController(StripeSettings stripeSettings)
    {
        _stripeSettings = stripeSettings;
    }

    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession([FromBody] CheckoutRequest request)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "eur",
                    UnitAmount = request.Amount,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Box Delivery Service"
                    }
                },
                Quantity = 1

            }
        },
            Mode = "payment",
            SuccessUrl = "https://localhost:7064/payment-success",
            CancelUrl = "https://localhost:7064/payment-cancel",
            CustomerEmail = request.Email, // Email for receipt (only in live/ manually send from https://dashboard.stripe.com/test/payments)
            InvoiceCreation = new SessionInvoiceCreationOptions
            {
                Enabled = true
            }
        };

        var service = new SessionService();
        var session = service.Create(options);

        return Ok(new { sessionUrl = session.Url });
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
                "whsec_0148e4dea24d9cf8a700f02bc099c9771967b55062ab92952221195512349da9"
            );

            switch (stripeEvent.Type)
            {
                case "invoice.finalized":
                    Console.WriteLine("Invoice finalized. Email should have been sent.");
                    break;

                case "payment_intent.succeeded":
                    Console.WriteLine("Payment succeeded. Email receipt should have been sent.");
                    break;

                default:
                    Console.WriteLine($"Unhandled event type: {stripeEvent.Type}");
                    break;
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


}