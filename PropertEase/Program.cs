using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using PropertEase.Hubs;
using Infrastructure.Identity;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Application.Interfaces;
using Application.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>();
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddDefaultIdentity<MyApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

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
            var userService = services.GetRequiredService<IUserService>();
            userService.SeedRolesAndUsers();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.MapHub<MessagingHub>("/messagingHub");

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }
}
