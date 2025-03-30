using Checkout_API_Final.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DB context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Bind to PORT environment variable for Render (or 5050 locally)
var port = Environment.GetEnvironmentVariable("PORT") ?? "5050";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

// 🔧 Environment-specific config
app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
{
    
    app.UseHttpsRedirection(); 
}

app.UseAuthorization();
app.MapControllers();

app.Run();
