using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using animatchWeb.Areas.Identity.Data;
using animatchWeb.Models;

namespace animatchWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Anime> Anime { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<AnimeGenre> AnimeGenre { get; set; }
        public DbSet<AddedAnime> AddedAnime { get; set; }
        public DbSet<LikedAnime> LikedAnime { get; set; }
        public DbSet<DislikedAnime> DislikedAnime { get; set; }
        public DbSet<WatchedAnime> WatchedAnime { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(modelBuilder);

            // Тут ви можете додати будь-які специфічні конфігурації для моделей та таблиць вашого додатку.
        }
    }
}
