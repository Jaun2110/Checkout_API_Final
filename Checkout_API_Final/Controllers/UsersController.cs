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
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Users

        [HttpPost]
        public async Task<IActionResult> CreateUser(string username)
        {
            var apiKey = Guid.NewGuid().ToString();
            var user = new User { ApiKey = apiKey, Username = username };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { ApiKey = apiKey });

        }


    }
}

        

        

       
    

