 Checkout API 

This is a Checkout API built with ASP.NET Core (.NET 8) and Entity Framework Core, using SQLite as the database provider.  
It allows users to register, manage products, and perform a checkout process with proper quantity and stock validation.

---

Live API

The API is publicly hosted at:

 [https://checkout-api-final-1.onrender.com](https://checkout-api-final-1.onrender.com)

Use tools like Postman or Swagger UI to interact with it.

---

##  Features

-  User registration with API key authentication
-  Product creation, listing, update & delete (by owner only)
-  View all products (public)
-  Checkout process:
  - Add/update/remove items from an open checkout
  - Quantity validation and stock management
  - One open checkout per user
  - Checkout summary and stock deduction on completion

---

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Visual Studio 2022+ or VS Code
- (Optional) Postman (for API testing)

---

## Setup Instructions (Local)

1. Clone the Repository
   ```bash
   git clone https://github.com/your-username/checkout-api-final.git
   cd checkout-api-final

2. Restore dependencies - run command: dotnet restore

3.Create the database - run command: dotnet ef database update

4. Run the application with command dotnet run


 Testing the API with Postman
 A Postman collection is included:
- CheckoutAPI.postman_collection.json

Use the following environment variables in Postman:

	
baseUrl:	https://checkout-api-final-1.onrender.com/api
apiKey: 	Set this after calling POST /users?username=...
 Authentication
This API uses a simple API key system:

[FromHeader] string apiKey
Each request requiring user-specific access must include the user's API key in the header.

Swagger UI

Swagger UI is available at:
 https://checkout-api-final-1.onrender.com/swagger

 Docker Support (for Render or Local Use)
 Build and run locally:

docker build -t checkout-api .
docker run -p 5050:5050 checkout-api

 Used for deployment on Render, with:
Custom Dockerfile

Dynamic port handling via PORT environment variable





