using GorodDeneg.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GorodDeneg.API.Data;

// IdentityDbContext<AppUser, IdentityRole<int>, int> — всё из Identity
public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category>      Categories     { get; set; }
    public DbSet<Project>       Projects       { get; set; }
    public DbSet<Reward>        Rewards        { get; set; }
    public DbSet<Pledge>        Pledges        { get; set; }
    public DbSet<Transaction>   Transactions   { get; set; }
    public DbSet<LegalDetails>  LegalDetails   { get; set; }
    public DbSet<ProjectUpdate> ProjectUpdates { get; set; }
    public DbSet<ProjectMedia>  ProjectMedia   { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // ── AppUser ──────────────────────────────────────
        b.Entity<AppUser>(e =>
        {
            e.ToTable("Users");
            e.Property(u => u.Balance).HasColumnType("decimal(18,2)");
        });

        // Rename Identity tables to cleaner names
        b.Entity<IdentityRole<int>>().ToTable("Roles");
        b.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        b.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        b.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        b.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        b.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

        // ── Category ─────────────────────────────────────
        b.Entity<Category>(e =>
        {
            e.HasIndex(c => c.Slug).IsUnique();
            e.HasIndex(c => c.Name).IsUnique();
        });

        // ── Project ──────────────────────────────────────
        b.Entity<Project>(e =>
        {
            e.HasOne(p => p.Author)
             .WithMany(u => u.Projects)
             .HasForeignKey(p => p.AuthorId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(p => p.Category)
             .WithMany(c => c.Projects)
             .HasForeignKey(p => p.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(p => p.GoalAmount).HasColumnType("decimal(18,2)");
            e.Property(p => p.CollectedAmount).HasColumnType("decimal(18,2)");
        });

        // ── Reward ───────────────────────────────────────
        b.Entity<Reward>(e =>
        {
            e.HasOne(r => r.Project)
             .WithMany(p => p.Rewards)
             .HasForeignKey(r => r.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(r => r.MinAmount).HasColumnType("decimal(18,2)");
        });

        // ── Pledge ───────────────────────────────────────
        b.Entity<Pledge>(e =>
        {
            e.HasOne(pl => pl.Project)
             .WithMany(p => p.Pledges)
             .HasForeignKey(pl => pl.ProjectId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(pl => pl.User)
             .WithMany(u => u.Pledges)
             .HasForeignKey(pl => pl.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(pl => pl.Reward)
             .WithMany(r => r.Pledges)
             .HasForeignKey(pl => pl.RewardId)
             .OnDelete(DeleteBehavior.SetNull);

            e.Property(pl => pl.Amount).HasColumnType("decimal(18,2)");
        });

        // ── Transaction ──────────────────────────────────
        b.Entity<Transaction>(e =>
        {
            e.HasOne(t => t.User)
             .WithMany(u => u.Transactions)
             .HasForeignKey(t => t.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            e.Property(t => t.BalanceBefore).HasColumnType("decimal(18,2)");
            e.Property(t => t.BalanceAfter).HasColumnType("decimal(18,2)");
        });

        // ── LegalDetails ─────────────────────────────────
        b.Entity<LegalDetails>(e =>
        {
            e.HasKey(l => l.UserId);
            e.HasOne(l => l.User)
             .WithOne(u => u.LegalDetails)
             .HasForeignKey<LegalDetails>(l => l.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ProjectUpdate ────────────────────────────────
        b.Entity<ProjectUpdate>(e =>
        {
            e.HasOne(u => u.Project)
             .WithMany(p => p.Updates)
             .HasForeignKey(u => u.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ProjectMedia ─────────────────────────────────
        b.Entity<ProjectMedia>(e =>
        {
            e.HasOne(m => m.Project)
             .WithMany(p => p.Media)
             .HasForeignKey(m => m.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Seed Data ────────────────────────────────────
        SeedCategories(b);
    }

    private static void SeedCategories(ModelBuilder b)
    {
        b.Entity<Category>().HasData(
            new Category { Id =  1, Name = "Бизнес",         Slug = "business",   Icon = "💼" },
            new Category { Id =  2, Name = "Дизайн",         Slug = "design",     Icon = "🎨" },
            new Category { Id =  3, Name = "Еда",            Slug = "food",       Icon = "🍽️" },
            new Category { Id =  4, Name = "Здоровье",       Slug = "health",     Icon = "❤️" },
            new Category { Id =  5, Name = "Игры",           Slug = "games",      Icon = "🎮" },
            new Category { Id =  6, Name = "Издание",        Slug = "publishing", Icon = "📚" },
            new Category { Id =  7, Name = "Искусство",      Slug = "art",        Icon = "🖼️" },
            new Category { Id =  8, Name = "Криптовалюты",   Slug = "crypto",     Icon = "₿"  },
            new Category { Id =  9, Name = "Мода",           Slug = "fashion",    Icon = "👗" },
            new Category { Id = 10, Name = "Музыка",         Slug = "music",      Icon = "🎵" },
            new Category { Id = 11, Name = "Образование",    Slug = "education",  Icon = "🎓" },
            new Category { Id = 12, Name = "Общество",       Slug = "community",  Icon = "🤝" },
            new Category { Id = 13, Name = "Спорт",          Slug = "sport",      Icon = "⚽" },
            new Category { Id = 14, Name = "Театр",          Slug = "theater",    Icon = "🎭" },
            new Category { Id = 15, Name = "Технологии",     Slug = "tech",       Icon = "💻" },
            new Category { Id = 16, Name = "Туризм",         Slug = "tourism",    Icon = "✈️" },
            new Category { Id = 17, Name = "Фильмы и видео", Slug = "film",       Icon = "🎬" },
            new Category { Id = 18, Name = "Фотография",     Slug = "photo",      Icon = "📷" },
            new Category { Id = 19, Name = "Хореография",    Slug = "dance",      Icon = "💃" }
        );
    }
}
