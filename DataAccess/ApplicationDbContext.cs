using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Agendly.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<Categorie> Categorias { get; set; }
        public DbSet<Comment> Comments { get; set; }    
        public DbSet<WishListEvent> WishListEvents { get; set; }
        public DbSet<WishList> wishLists { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<LikeDislike> LikeDislikes { get; set; }
        public DbSet<Notification> notifications { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // الجداول الخاصة بـ Identity
            builder.Entity<ApplicationUser>().ToTable("Users", "Security");
            builder.Entity<IdentityRole>().ToTable("Roles", "Security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole", "Security");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim", "Security");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken", "Security");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin", "Security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim", "Security");

            // تعيين القيد بين الـ User و WishList مع تحديد سلوك الحذف
            builder.Entity<WishList>()
                .HasOne(w => w.User) // العلاقة بين WishList و User
                .WithMany(u => u.WishLists) // المستخدم يمكن أن يكون له عدة wishLists
                .HasForeignKey(w => w.UserId) // المفتاح الأجنبي في WishList
                .OnDelete(DeleteBehavior.NoAction);  // لا يتم حذف الـ WishList إذا تم حذف المستخدم

            // تعيين القيد بين الـ User و Bookings مع تحديد سلوك الحذف
            builder.Entity<Booking>()
                .HasOne(b => b.User) // العلاقة بين Booking و User
                .WithMany(u => u.Bookings) // المستخدم يمكن أن يكون له عدة Bookings
                .HasForeignKey(b => b.UserId) // المفتاح الأجنبي في Booking
                .OnDelete(DeleteBehavior.NoAction);  // لا يتم حذف الـ Booking إذا تم حذف المستخدم

            // تعيين القيد بين الـ User و Events مع تحديد سلوك الحذف
            builder.Entity<Event>()
                .HasOne(e => e.User) // العلاقة بين Event و User
                .WithMany(u => u.Events) // المستخدم يمكن أن يكون له عدة Events
                .HasForeignKey(e => e.UserId) // المفتاح الأجنبي في Event
                .OnDelete(DeleteBehavior.NoAction);  // لا يتم حذف الـ Event إذا تم حذف المستخدم
        }


    }
}
