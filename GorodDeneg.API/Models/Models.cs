using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GorodDeneg.API.Models;

// ─── Identity User (расширяем IdentityUser) ───────────────────────
public class AppUser : IdentityUser<int>
{
    [MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)] public string LastName  { get; set; } = string.Empty;
    [MaxLength(100)] public string? City     { get; set; }
    [MaxLength(255)] public string? Address  { get; set; }
    public DateTime?              DateOfBirth { get; set; }
    [MaxLength(1000)] public string? Bio     { get; set; }
    [MaxLength(500)]  public string? AvatarUrl { get; set; }
    [MaxLength(255)]  public string? WebSite  { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Balance { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Project>     Projects     { get; set; } = new List<Project>();
    public ICollection<Pledge>      Pledges      { get; set; } = new List<Pledge>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public LegalDetails?            LegalDetails { get; set; }
}

// ─── Category ──────────────────────────────────────────────────────
public class Category
{
    public int    Id   { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(100)] public string Slug { get; set; } = string.Empty;
    [MaxLength(10)]  public string? Icon { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}

// ─── Project ───────────────────────────────────────────────────────
public class Project
{
    public int    Id         { get; set; }
    public int    AuthorId   { get; set; }
    public int    CategoryId { get; set; }

    [MaxLength(255)] public string Title     { get; set; } = string.Empty;
    [MaxLength(500)] public string ShortDesc { get; set; } = string.Empty;
    public string? FullDesc     { get; set; }
    [MaxLength(500)] public string? CoverImageUrl { get; set; }
    [MaxLength(500)] public string? VideoUrl      { get; set; }
    [MaxLength(100)] public string? City          { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal GoalAmount       { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal CollectedAmount  { get; set; } = 0;

    public int        DurationDays  { get; set; }
    public DateTime?  StartDate     { get; set; }
    public DateTime?  EndDate       { get; set; }

    // Draft | Pending | Active | Rejected | Completed | Failed
    [MaxLength(30)] public string Status { get; set; } = "Draft";

    public bool IsHidden    { get; set; } = false;
    [MaxLength(20)] public string? PackageType { get; set; }   // Start | Advanced
    public bool PackagePaid { get; set; } = false;
    public int  SortPriority { get; set; } = 0;
    public int  ViewsCount   { get; set; } = 0;
    public int  BackersCount { get; set; } = 0;
    [MaxLength(1000)] public string? AdminNote { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser            Author   { get; set; } = null!;
    public Category           Category { get; set; } = null!;
    public ICollection<Reward>        Rewards  { get; set; } = new List<Reward>();
    public ICollection<Pledge>        Pledges  { get; set; } = new List<Pledge>();
    public ICollection<ProjectMedia>  Media    { get; set; } = new List<ProjectMedia>();
    public ICollection<ProjectUpdate> Updates  { get; set; } = new List<ProjectUpdate>();
}

// ─── Reward ────────────────────────────────────────────────────────
public class Reward
{
    public int ProjectId { get; set; }
    public int Id        { get; set; }
    [MaxLength(255)]  public string  Title       { get; set; } = string.Empty;
    [MaxLength(1000)] public string? Description { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal MinAmount { get; set; }
    public int?  DeliveryMonth { get; set; }
    public int?  DeliveryYear  { get; set; }
    [MaxLength(50)]  public string ShippingType { get; set; } = "Не требует доставки";
    [MaxLength(500)] public string? ImageUrl    { get; set; }
    public int? LimitCount   { get; set; }
    public int  ClaimedCount { get; set; } = 0;
    public bool IsActive     { get; set; } = true;
    public int  SortOrder    { get; set; } = 0;

    public Project Project { get; set; } = null!;
    public ICollection<Pledge> Pledges { get; set; } = new List<Pledge>();
}

// ─── Pledge ────────────────────────────────────────────────────────
public class Pledge
{
    public int     Id        { get; set; }
    public int     ProjectId { get; set; }
    public int     UserId    { get; set; }
    public int?    RewardId  { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Amount { get; set; }
    // Pending | Paid | Refunded | Failed
    [MaxLength(20)] public string Status { get; set; } = "Pending";
    [MaxLength(100)] public string? PaymentRef     { get; set; }
    [MaxLength(500)] public string? ShippingAddress { get; set; }
    public DateTime  CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt    { get; set; }

    public Project  Project { get; set; } = null!;
    public AppUser  User    { get; set; } = null!;
    public Reward?  Reward  { get; set; }
}

// ─── Transaction ───────────────────────────────────────────────────
public class Transaction
{
    public int    Id     { get; set; }
    public int    UserId { get; set; }
    // TopUp | PledgeOut | Refund | PlatformFee
    [MaxLength(50)] public string Type { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18,2)")] public decimal Amount        { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal BalanceBefore { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal BalanceAfter  { get; set; }
    public int?   RefId { get; set; }
    [MaxLength(255)] public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppUser User { get; set; } = null!;
}

// ─── LegalDetails ──────────────────────────────────────────────────
public class LegalDetails
{
    public int UserId { get; set; }
    // Individual | Organization | IE
    [MaxLength(30)]  public string LegalType  { get; set; } = string.Empty;
    [MaxLength(20)]  public string? IIN        { get; set; }
    [MaxLength(20)]  public string? BIN        { get; set; }
    [MaxLength(255)] public string? BankName   { get; set; }
    [MaxLength(20)]  public string? BIK        { get; set; }
    [MaxLength(50)]  public string? AccountNumber { get; set; }
    [MaxLength(255)] public string? FullName   { get; set; }
    [MaxLength(500)] public string? RegAddress { get; set; }
    [MaxLength(255)] public string? OrgName    { get; set; }
    [MaxLength(500)] public string? OrgLegalAddress { get; set; }
    [MaxLength(500)] public string? OrgFactAddress  { get; set; }
    [MaxLength(255)] public string? OrgDirector     { get; set; }
    [MaxLength(500)] public string? IINFrontUrl     { get; set; }
    [MaxLength(500)] public string? IINBackUrl       { get; set; }
    [MaxLength(500)] public string? RegCertUrl      { get; set; }
    [MaxLength(50)]  public string Country { get; set; } = "Kazakhstan";
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public AppUser User { get; set; } = null!;
}

// ─── ProjectUpdate ─────────────────────────────────────────────────
public class ProjectUpdate
{
    public int    Id        { get; set; }
    public int    ProjectId { get; set; }
    [MaxLength(255)] public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project Project { get; set; } = null!;
}

// ─── ProjectMedia ──────────────────────────────────────────────────
public class ProjectMedia
{
    public int    Id        { get; set; }
    public int    ProjectId { get; set; }
    [MaxLength(10)]  public string MediaType { get; set; } = "image"; // image | video
    [MaxLength(500)] public string Url       { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;

    public Project Project { get; set; } = null!;
}
