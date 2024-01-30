using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelPlan.Models.Entities;

namespace TravelPlan.Data
{
    public class DatabaseContext : IdentityDbContext<User,Role,string>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

		public DbSet<Settings> Settings { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Ads> Ads { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
