using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Checkout_API_Final.Data;
using Checkout_API_Final.Models;
using NuGet.Protocol;

namespace Checkout_API_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }
        //add a product
        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product, [FromHeader] string apiKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
            if (user == null) return Unauthorized();

            product.UserId = user.Id;
            _context.Add(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product updatedProduct, [FromHeader] string apiKey)
        {
        //Find the user based on the api key
        var user = await _context.Users.FirstOrDefaultAsync(u=> u.ApiKey == apiKey);
        if (user == null) return Unauthorized();    
        //Find the product in the db
        var product = await _context.Products.FirstOrDefaultAsync(p=> p.Id == id);
            if (product == null) return NotFound("Product not found.");

            if (product.UserId != user.Id)
                return Unauthorized("You can’t modify a product you didn’t create.");

            //update product values
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;

            //save to db
            await _context.SaveChangesAsync();
            return Ok(product); 
        }
//endpoint to delete a product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, [FromHeader] string apiKey)
        {
            // Find the user
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.ApiKey == apiKey);
            if (user == null) return Unauthorized();

            //Find the product
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound("Product not found.");

            //check if the user owns the product
            if (product.UserId != user.Id)
                return Unauthorized("You do not have permission to delete this product.");

            //delete and save
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();  
            return Ok($"Product with id {id} has been deleted.");
        }
// return a list of all products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products.Include(p => p.User).ToListAsync();
            return Ok(products);
        }
       

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
