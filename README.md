# 🧾 Checkout API (.NET 8 Web API)

This is a simple Checkout API built with ASP.NET Core  and Entity Framework Core using SQLite as the database provider.  
It allows users to register, manage products, and perform a checkout with quantity/stock validation.

---

 Features

- User registration with API key
- Product CRUD (Create, Read, Update, Delete)
- Secure product ownership (only owners can update/delete)
- Public product listing
- Checkout process with:
  - Add/update/remove products from checkout
  - One open checkout per user
  - Quantity validation and stock deduction
  - Complete checkout with totals


 Requirements

- [.NET 8 SDK]
- Visual Studio 2022+ or VS Code
- (Optional) Postman for testing API endpoints

---

 Setup Instructions

1. Clone the Repository
2. Restore dependdencies
3. Create the database using EF Core Migrations
4. Run the app, the API will start on http://localhost:5050

Testing the API with Postman:

1. I have included a Postman collection in the project that can be imported to test all endpoints.
2. use environment variables:
baseUrl = http://localhost:5050/api

apiKey = (set after creating a user)

For authentication i used [FromHeader] string apikey instead of ASP.Net auth.

NB: apublicly available version of this api is at: url
