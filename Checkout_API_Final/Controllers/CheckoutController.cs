using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Checkout_API_Final.Data;
using Checkout_API_Final.Models;

namespace Checkout_API_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CheckoutController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartCheckout([FromBody] List<ChecoutItemRequest>items, [FromHeader] string apiKey)
        {
            //Find the user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
            if (user == null) return Unauthorized("You do not have authoristion to checkout this product.");

            //Check for existing checkouts
            var existingCheckout = await _context.Checkouts.FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsComplete);
            if (existingCheckout != null) return BadRequest("You already have an open checkout.");

            //create a new checkout instance
            var checkout = new Checkout
            {
                UserId = user.Id,
                IsComplete = false,
                Items = new List<CheckoutItem>()
            };

            foreach (var item in items) 
            {
                if (item.Quantity <= 0)
                    return BadRequest($"Quantity must be greater than 0 for product ID {item.ProductId}");

                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"Invalid product ID {item.ProductId}");

                if (product.Quantity < item.Quantity)
                    return BadRequest($"Not enough stock for product ID {item.ProductId}");

                checkout.Items.Add(new CheckoutItem {
                    ProductId = product.Id, 
                    Quantity = item.Quantity,
                    PriceAtTime = product.Price
                });  
            }
            _context.Checkouts.Add(checkout);
            await _context.SaveChangesAsync();
            var summary = checkout.Items.Select(i => new
            {
                i.ProductId,
                i.Quantity,
                i.PriceAtTime,
                Subtotal = i.Quantity * i.PriceAtTime
            }).ToList();

            var total = summary.Sum(i => i.Subtotal);

            return Ok(new
            {
                Message = "Checkout started successfully",
                CheckoutId = checkout.Id,
                Items = summary,
                Total = total
            });
        }
        

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteCheckout([FromHeader] string apiKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
            if (user == null) return Unauthorized();

            var checkout = await _context.Checkouts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsComplete);
            if (checkout == null) return BadRequest("No open checkout found.");

            var summary = new List<object>();
            decimal total = 0;

            foreach (var item in checkout.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"Product with ID {item.ProductId} no longer exists.");

                if (product.Quantity < item.Quantity)
                    return BadRequest($"Not enough stock for product ID {product.Id}");

                product.Quantity -= item.Quantity;

                var subtotal = item.Quantity * item.PriceAtTime;
                total += subtotal;

                summary.Add(new
                {
                    item.ProductId,
                    item.Quantity,
                    item.PriceAtTime,
                    Subtotal = subtotal
                });
            }

            checkout.IsComplete = true;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Checkout completed successfully!",
                CheckoutId = checkout.Id,
                Items = summary,
                Total = total
            });
        }


        //update product quantity before checkout is complete
        [HttpPut("update-item")]
        public async Task<IActionResult> UpdateItemQuantity([FromBody] UpdateItemRequest req, [FromHeader] string apiKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
            if (user == null) return Unauthorized("You are not authorised to update this product.");

            var checkout = await _context.Checkouts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsComplete);

            if (checkout == null)
                return NotFound("No open checkout found.");

            var item = checkout.Items.FirstOrDefault(i => i.ProductId == req.ProductId);
            if (item == null)
                return NotFound("Product not found in your checkout.");

            if (req.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            item.Quantity = req.Quantity;

            await _context.SaveChangesAsync();
            return Ok($"Quantity for product {req.ProductId} updated to {req.Quantity}.");
        }

        //remove product from open checkout
        [HttpDelete("remove-item/{productId}")]
        public async Task<IActionResult> RemoveItemFromCheckout(int productId, [FromHeader] string apiKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
            if (user == null) return Unauthorized("You are not authorised to delete this product.");

            var checkout = await _context.Checkouts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsComplete);

            if (checkout == null)
                return NotFound("No open checkout found.");

            var item = checkout.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                return NotFound($"Product ID {productId} not found in your checkout.");

            checkout.Items.Remove(item);

            //If the checkout is empty, remove it
            if (!checkout.Items.Any())
            {
                _context.Checkouts.Remove(checkout);
                await _context.SaveChangesAsync();
                return Ok("Product removed. Checkout is now empty and has been deleted.");
            }

            await _context.SaveChangesAsync();
            return Ok($"Product ID {productId} removed from checkout.");
        }


    }
}
