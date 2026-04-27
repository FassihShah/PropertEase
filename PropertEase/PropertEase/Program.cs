using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertEase.Hubs;
using Infrastructure.Identity;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        var databaseProvider = builder.Configuration["Database:Provider"] ?? "SqlServer";

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                EnsureSqliteDirectoryExists(connectionString);
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseSqlServer(connectionString);
            }
        });
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddDefaultIdentity<MyApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        var dataProtectionKeysPath = builder.Configuration["DataProtection:KeysPath"];
        if (!string.IsNullOrWhiteSpace(dataProtectionKeysPath))
        {
            Directory.CreateDirectory(dataProtectionKeysPath);
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));
        }

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IImageRepository, ImageRepository>();
        builder.Services.AddScoped<ILocationRepository, LocationRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<IPropertyPurposeRepository, PropertyPurposeRepository>();
        builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
        builder.Services.AddScoped<IPropertyTypeRepository, PropertyTypeRepository>();

        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IImageService, ImageService>();
        builder.Services.AddScoped<ILocationService, LocationService>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<IPropertyPurposeService, PropertyPurposeService>();
        builder.Services.AddScoped<IPropertyService, PropertyService>();
        builder.Services.AddScoped<IPropertyTypeService, PropertyTypeService>();
        builder.Services.AddScoped<IUserService, UserService>();


        builder.Services.AddControllersWithViews();
        builder.Services.AddSignalR();

        var app = builder.Build();

        // Seed roles and users
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            if (builder.Configuration.GetValue<bool>("Database:EnsureCreated"))
            {
                await dbContext.Database.EnsureCreatedAsync();
            }
            else if (builder.Configuration.GetValue<bool>("Database:AutoMigrate"))
            {
                await dbContext.Database.MigrateAsync();
            }

            var userService = services.GetRequiredService<IUserService>();
            await userService.SeedRolesAndUsers();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            if (builder.Configuration.GetValue<bool>("Security:UseHsts"))
            {
                app.UseHsts();
            }
        }
        if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Security:UseHttpsRedirection"))
        {
            app.UseHttpsRedirection();
        }
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<MessagingHub>("/messagingHub");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }

    private static void EnsureSqliteDirectoryExists(string connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        var dataSource = builder.DataSource;
        if (string.IsNullOrWhiteSpace(dataSource) || dataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var directory = Path.GetDirectoryName(Path.GetFullPath(dataSource));
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
