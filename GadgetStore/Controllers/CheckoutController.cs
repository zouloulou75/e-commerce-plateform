using GadgetStore.Data;
using GadgetStore.Models;
using GadgetStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using GadgetStore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GadgetStore.Controllers
{
    [Authorize]
    [Route("checkout")]
    public class CheckoutController : Controller
    {
        private readonly GadgetStoreDbContext _context;

        public CheckoutController(GadgetStoreDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (!cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var total = cart.Sum(item => item.Product.Price * item.Quantity);
            ViewBag.Total = total;
            return View(cart);
        }
        [HttpPost("process")]
        public async Task<IActionResult> ProcessOrder(string customerName, string email, string address, string phone, string paymentMethod)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (!cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                CustomerName = customerName,
                Email = email,
                Address = address,
                Phone = phone,
                PaymentMethod = paymentMethod,
                TotalAmount = cart.Sum(item => item.Product.Price * item.Quantity),
                OrderItems = cart.Select(item => new OrderItem
                {
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            try
            {
                var emailService = HttpContext.RequestServices.GetRequiredService<IEmailService>();
                await emailService.SendOrderConfirmationAsync(
                    email,
                    customerName,
                    order.Id,
                    order.TotalAmount
                );
            }
            catch (Exception ex)
            {
                // Log the error but don't prevent the order from being placed
            }

            // Clear the cart
            HttpContext.Session.Remove("Cart");

            TempData["SuccessMessage"] = $"Thank you, {customerName}! Your order has been placed. A confirmation email has been sent to {email}.";
            return RedirectToAction("Index", "Products");
        }

    }
}
