using Microsoft.EntityFrameworkCore;
using animatchWeb.Models;
using System;

namespace animatchWeb.Data
{
    public class ApplicationDbContext : DbContext
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
    }
}
