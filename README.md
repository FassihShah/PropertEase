# PropertEase

PropertEase is an ASP.NET Core MVC real estate marketplace for listing, searching, and messaging about properties. The solution is organized into Domain, Application, Infrastructure, and MVC presentation projects.

## Features

- Property listings with images, pricing, size, type, category, purpose, and location
- Search and filtering by city, category, type, purpose, price, and size
- ASP.NET Core Identity authentication with User, Agent, and Admin roles
- Admin dashboard for users, agents, admins, and properties
- Buyer-to-seller messaging with SignalR notification support

## Tech Stack

- ASP.NET Core MVC and Razor Views
- Entity Framework Core 8
- ASP.NET Core Identity
- SQL Server
- SignalR
- Bootstrap, jQuery, and custom CSS

## Local Setup

1. Install the .NET 8 SDK.
2. Configure the database connection string:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=PropertEase;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

3. Set an admin seed password with user secrets or an environment variable:

   ```powershell
   dotnet user-secrets set "SeedAdmin:Password" "Use-A-Strong-Deployment-Password" --project PropertEase/PropertEase/PropertEase.csproj
   ```

4. Apply migrations and run the app:

   ```powershell
   dotnet ef database update --project PropertEase/Infrastructure/Infrastructure.csproj --startup-project PropertEase/PropertEase/PropertEase.csproj
   dotnet run --project PropertEase/PropertEase/PropertEase.csproj
   ```

## Deployment Notes

- Override `ConnectionStrings:DefaultConnection` in the hosting environment.
- Set `SeedAdmin:Password` as a secret in the hosting platform. The app will not seed the default admin account unless this value exists.
- Do not commit `bin/`, `obj/`, `.user`, uploaded files, or local development settings.
