using System.ComponentModel.DataAnnotations;

namespace GorodDeneg.API.DTOs;

// ── Auth ──────────────────────────────────────────────────────────
public record RegisterDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required, MaxLength(100)] string FirstName,
    [Required, MaxLength(100)] string LastName,
    string? Phone
);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponseDto(
    string AccessToken,
    UserDto User
);

// ── User ──────────────────────────────────────────────────────────
public class UserDto
{
    public int      Id         { get; set; }
    public string   Email      { get; set; } = string.Empty;
    public string   FirstName  { get; set; } = string.Empty;
    public string   LastName   { get; set; } = string.Empty;
    public string?  Phone      { get; set; }
    public string?  City       { get; set; }
    public string?  Address    { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string?  Bio        { get; set; }
    public string?  AvatarUrl  { get; set; }
    public string?  WebSite    { get; set; }
    public decimal  Balance    { get; set; }
    public string   Role       { get; set; } = "User";
    public bool     IsEmailConfirmed { get; set; }
    public DateTime CreatedAt  { get; set; }
}

public class UpdateProfileDto
{
    [MaxLength(100)] public string?   FirstName   { get; set; }
    [MaxLength(100)] public string?   LastName    { get; set; }
    [MaxLength(20)]  public string?   Phone       { get; set; }
    [MaxLength(100)] public string?   City        { get; set; }
    [MaxLength(255)] public string?   Address     { get; set; }
    public DateTime? DateOfBirth { get; set; }
    [MaxLength(1000)] public string?  Bio         { get; set; }
    [MaxLength(255)]  public string?  WebSite     { get; set; }
}

public record ChangePasswordDto(
    [Required] string CurrentPassword,
    [Required, MinLength(8)] string NewPassword
);

// ── Category ──────────────────────────────────────────────────────
public record CategoryDto(int Id, string Name, string Slug, string? Icon);

// ── Projects ──────────────────────────────────────────────────────
public class ProjectListDto
{
    public int      Id              { get; set; }
    public string   Title           { get; set; } = string.Empty;
    public string   ShortDesc       { get; set; } = string.Empty;
    public string?  CoverImageUrl   { get; set; }
    public string?  City            { get; set; }
    public decimal  GoalAmount      { get; set; }
    public decimal  CollectedAmount { get; set; }
    public int      BackersCount    { get; set; }
    public DateTime? EndDate        { get; set; }
    public string   Status          { get; set; } = string.Empty;
    public int      SortPriority    { get; set; }
    public string?  CategoryName    { get; set; }
    public string?  CategoryIcon    { get; set; }
    public int      DaysLeft        { get; set; }
    public int      PercentFunded   { get; set; }
}

public class ProjectDetailDto : ProjectListDto
{
    public string?   FullDesc   { get; set; }
    public string?   VideoUrl   { get; set; }
    public int       ViewsCount { get; set; }
    public DateTime  CreatedAt  { get; set; }
    public string?   AuthorName   { get; set; }
    public string?   AuthorAvatar { get; set; }
    public string?   AuthorBio    { get; set; }
    public List<RewardDto>        Rewards { get; set; } = new();
    public List<ProjectMediaDto>  Media   { get; set; } = new();
    public List<ProjectUpdateDto> Updates { get; set; } = new();
}

public class CreateProjectDto
{
    [Required, MaxLength(255)] public string Title     { get; set; } = string.Empty;
    [Required, MaxLength(500)] public string ShortDesc { get; set; } = string.Empty;
    public string? FullDesc  { get; set; }
    public string? VideoUrl  { get; set; }
    [Required] public int    CategoryId   { get; set; }
    [MaxLength(100)] public string? City  { get; set; }
    [Range(1000, 100_000_000)] public decimal GoalAmount  { get; set; }
    [Range(1, 90)]             public int     DurationDays { get; set; }
    public bool IsHidden { get; set; } = false;
    public List<CreateRewardDto> Rewards { get; set; } = new();
}

public class CreateRewardDto
{
    [Required, MaxLength(255)] public string  Title       { get; set; } = string.Empty;
    [MaxLength(1000)]          public string? Description { get; set; }
    [Range(100, 100_000_000)]  public decimal MinAmount   { get; set; }
    public int?    DeliveryMonth { get; set; }
    public int?    DeliveryYear  { get; set; }
    public string  ShippingType  { get; set; } = "Не требует доставки";
    public int?    LimitCount    { get; set; }
}

public class RewardDto
{
    public int     Id           { get; set; }
    public string  Title        { get; set; } = string.Empty;
    public string? Description  { get; set; }
    public decimal MinAmount    { get; set; }
    public int?    DeliveryMonth { get; set; }
    public int?    DeliveryYear  { get; set; }
    public string  ShippingType  { get; set; } = string.Empty;
    public string? ImageUrl      { get; set; }
    public int?    LimitCount    { get; set; }
    public int     ClaimedCount  { get; set; }
    public bool    IsActive      { get; set; }
}

public class ProjectMediaDto
{
    public int    Id        { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public string Url       { get; set; } = string.Empty;
    public int    SortOrder { get; set; }
}

public class ProjectUpdateDto
{
    public int      Id        { get; set; }
    public string   Title     { get; set; } = string.Empty;
    public string   Body      { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class SelectPackageDto
{
    [Required] public string PackageType { get; set; } = string.Empty; // Start | Advanced
    [Required] public string PaymentRef  { get; set; } = string.Empty;
}

// ── Pledges ───────────────────────────────────────────────────────
public class CreatePledgeDto
{
    public int?    RewardId        { get; set; }
    [Range(100, 100_000_000)] public decimal Amount { get; set; }
    [MaxLength(500)] public string? ShippingAddress { get; set; }
}

public class PledgeResultDto
{
    public int     PledgeId   { get; set; }
    public decimal NewBalance { get; set; }
    public string  Message    { get; set; } = string.Empty;
}

public class PledgeDto
{
    public int      Id           { get; set; }
    public int      ProjectId    { get; set; }
    public string?  ProjectTitle { get; set; }
    public string?  RewardTitle  { get; set; }
    public decimal  Amount       { get; set; }
    public string   Status       { get; set; } = string.Empty;
    public DateTime CreatedAt    { get; set; }
    public DateTime? PaidAt      { get; set; }
}

// ── Balance / Transactions ────────────────────────────────────────
public class TopUpDto
{
    [Range(100, 10_000_000)] public decimal Amount     { get; set; }
    [Required] public string PaymentRef { get; set; } = string.Empty;
}

public class TransactionDto
{
    public int      Id            { get; set; }
    public string   Type          { get; set; } = string.Empty;
    public decimal  Amount        { get; set; }
    public decimal  BalanceBefore { get; set; }
    public decimal  BalanceAfter  { get; set; }
    public string?  Note          { get; set; }
    public DateTime CreatedAt     { get; set; }
}

// ── Legal ─────────────────────────────────────────────────────────
public class SaveLegalDto
{
    [Required] public string  LegalType     { get; set; } = string.Empty;
    public string? IIN           { get; set; }
    public string? BIN           { get; set; }
    public string? BankName      { get; set; }
    public string? BIK           { get; set; }
    public string? AccountNumber { get; set; }
    public string? FullName      { get; set; }
    public string? RegAddress    { get; set; }
    public string? OrgName       { get; set; }
    public string? OrgLegalAddress { get; set; }
    public string? OrgFactAddress  { get; set; }
    public string? OrgDirector     { get; set; }
}

// ── Admin ─────────────────────────────────────────────────────────
public class AdminProjectDto
{
    public int      Id              { get; set; }
    public string   Title           { get; set; } = string.Empty;
    public decimal  GoalAmount      { get; set; }
    public decimal  CollectedAmount { get; set; }
    public string   Status          { get; set; } = string.Empty;
    public string?  AdminNote       { get; set; }
    public string?  AuthorEmail     { get; set; }
    public string?  CategoryName    { get; set; }
    public DateTime CreatedAt       { get; set; }
    public int      BackersCount    { get; set; }
    public int      PercentFunded   { get; set; }
}

public class UpdateStatusDto
{
    [Required] public string  Status    { get; set; } = string.Empty;
    public string? AdminNote { get; set; }
}

public class AdminStatsDto
{
    public int     ActiveProjects  { get; set; }
    public int     PendingProjects { get; set; }
    public int     TotalUsers      { get; set; }
    public decimal MonthlyRevenue  { get; set; }
    public int     MonthlyPledges  { get; set; }
}

// ── Pagination ────────────────────────────────────────────────────
public class PagedResult<T>
{
    public List<T> Items      { get; set; } = new();
    public int     TotalCount { get; set; }
    public int     Page       { get; set; }
    public int     PageSize   { get; set; }
    public int     TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool    HasNext    => Page < TotalPages;
    public bool    HasPrev    => Page > 1;
}
