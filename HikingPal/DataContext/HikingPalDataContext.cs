
using HikingPal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HikingPal.DataContext
{
    public class HikingPalDataContext : DbContext
    {
        private readonly DbConnectionStrings _connectionStrings;

        public HikingPalDataContext(IOptions<DbConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionStrings.MSSQLDatabase);            
        }       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users).HasForeignKey("RoleID").OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Hike>()
                .HasOne(h => h.Author)
                .WithMany(u => u.HikeAutors).HasForeignKey("AuthorID");

            modelBuilder.Entity<HikeUser>()
                .HasKey(hu => hu.HikeUserID);

            modelBuilder.Entity<HikeUser>()
                .HasOne(hu => hu.Hike)
                .WithMany(h => h.HikeUsers)
                .HasForeignKey(hu => hu.HikeID).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HikeUser>()
                .HasOne(hu => hu.User)
                .WithMany(u => u.HikeUsers)
                .HasForeignKey(hu => hu.UserID).OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Role>().HasData(new Role()
            {
                RoleID = Guid.NewGuid(),
                RoleName = "Admin",
                RoleDescription = "Rules everythings."
            },
            new Role()
            {
                RoleID = Guid.NewGuid(),
                RoleName = "Hiker",
                RoleDescription = "A simple Hiker."
            },
            new Role()
            {
                RoleID = Guid.NewGuid(),
                RoleName = "Organiser",
                RoleDescription = "A simple organiser who can organise hiking."
            });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Hike> Hikes { get; set; }
        public DbSet<HikeUser> HikeUsers { get; set; }

    }
}
