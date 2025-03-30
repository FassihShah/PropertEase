using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Identity
{
    public class ApplicationDbContext : IdentityDbContext<MyApplicationUser>
	{
        public DbSet<Property> Properties { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PropertyPurpose> PropertyPurposes { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { 
        
        }
        public ApplicationDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=PropertEase;Integrated Security=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PropertyPurpose>().HasData(
                new PropertyPurpose { PropertyPurposeId = 1, Name = "Sale" },
                new PropertyPurpose { PropertyPurposeId = 2, Name = "Rent" }
            );

            modelBuilder.Entity<PropertyType>().HasData(
                new PropertyType { PropertyTypeId = 1, Name = "House" },
                new PropertyType { PropertyTypeId = 2, Name = "Apartment" },
                new PropertyType { PropertyTypeId = 3, Name = "Plot" },
                new PropertyType { PropertyTypeId = 4, Name = "Shop" },
                new PropertyType { PropertyTypeId = 5, Name = "Office" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Residential" },
                new Category { CategoryId = 2, Name = "Commercial" },
                new Category { CategoryId = 3, Name = "Industrial" }
            );

            // Define relationships for Property
            modelBuilder.Entity<Property>()
                .HasOne(p => p.PropertyType)
                .WithMany()
                .HasForeignKey("PropertyTypeId");

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey("CategoryId");

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Purpose)
                .WithMany()
                .HasForeignKey("PurposeId");

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Location)
                .WithOne(l => l.Property)
                .HasForeignKey<Location>(l => l.PropertyId);

            modelBuilder.Entity<Property>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId);
        }
    }
    
}
