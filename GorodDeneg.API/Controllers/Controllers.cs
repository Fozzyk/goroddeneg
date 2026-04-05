using AutoMapper;
using GorodDeneg.API.Data;
using GorodDeneg.API.DTOs;
using GorodDeneg.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GorodDeneg.API.Services;

namespace GorodDeneg.API.Controllers;

// ════════════════════════════════════════════════════════════════════
// AUTH
// ════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _users;
    private readonly SignInManager<AppUser> _signIn;
    private readonly ITokenService _tokens;
    private readonly IMapper _mapper;

    public AuthController(UserManager<AppUser> users, SignInManager<AppUser> signIn,
        ITokenService tokens, IMapper mapper)
    {
        _users = users; _signIn = signIn; _tokens = tokens; _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _users.FindByEmailAsync(dto.Email) != null)
            return Conflict(new { message = "Пользователь с таким email уже зарегистрирован" });

        var user = new AppUser
        {
            UserName  = dto.Email,
            Email     = dto.Email,
            FirstName = dto.FirstName,
            LastName  = dto.LastName,
            PhoneNumber = dto.Phone,
            EmailConfirmed = true   // упрощаем для демо
        };

        var result = await _users.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        await _users.AddToRoleAsync(user, "User");

        var token = await _tokens.GenerateTokenAsync(user);
        var dto2  = _mapper.Map<UserDto>(user);
        dto2.Role  = "User";
        return Ok(new AuthResponseDto(token, dto2));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _users.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized(new { message = "Неверный email или пароль" });

        var result = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Неверный email или пароль" });

        var token  = await _tokens.GenerateTokenAsync(user);
        var roles  = await _users.GetRolesAsync(user);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Role = roles.FirstOrDefault() ?? "User";
        return Ok(new AuthResponseDto(token, userDto));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _users.GetUserAsync(User);
        if (user == null) return NotFound();
        var roles  = await _users.GetRolesAsync(user);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Role = roles.FirstOrDefault() ?? "User";
        return Ok(userDto);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var user = await _users.GetUserAsync(User);
        if (user == null) return NotFound();
        var result = await _users.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        return Ok(new { message = "Пароль изменён успешно" });
    }
}

// ════════════════════════════════════════════════════════════════════
// CATEGORIES
// ════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public CategoriesController(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cats = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        return Ok(_mapper.Map<List<CategoryDto>>(cats));
    }
}

// ════════════════════════════════════════════════════════════════════
// PROJECTS
// ════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _users;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public ProjectsController(AppDbContext db, UserManager<AppUser> users,
        IMapper mapper, IWebHostEnvironment env)
    {
        _db = db; _users = users; _mapper = mapper; _env = env;
    }

    // GET /api/projects?categoryId=1&search=eco&page=1&pageSize=12
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId, [FromQuery] string? search,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        var query = _db.Projects
            .Include(p => p.Category)
            .Where(p => !p.IsHidden && p.Status == "Active");

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Title.Contains(search) || p.ShortDesc.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.SortPriority)
            .ThenByDescending(p => p.CollectedAmount)
            .ThenBy(p => p.EndDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PagedResult<ProjectListDto>
        {
            Items      = _mapper.Map<List<ProjectListDto>>(items),
            TotalCount = total,
            Page       = page,
            PageSize   = pageSize
        });
    }

    // GET /api/projects/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _db.Projects
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Rewards.Where(r => r.IsActive).OrderBy(r => r.SortOrder))
            .Include(p => p.Media.OrderBy(m => m.SortOrder))
            .Include(p => p.Updates.OrderByDescending(u => u.CreatedAt))
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return NotFound();

        // increment views
        project.ViewsCount++;
        await _db.SaveChangesAsync();

        return Ok(_mapper.Map<ProjectDetailDto>(project));
    }

    // GET /api/projects/my
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var userId = GetUserId();
        var projects = await _db.Projects
            .Include(p => p.Category)
            .Where(p => p.AuthorId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return Ok(_mapper.Map<List<ProjectListDto>>(projects));
    }

    // POST /api/projects
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetUserId();
        var project = new Project
        {
            AuthorId    = userId,
            CategoryId  = dto.CategoryId,
            Title       = dto.Title,
            ShortDesc   = dto.ShortDesc,
            FullDesc    = dto.FullDesc,
            VideoUrl    = dto.VideoUrl,
            City        = dto.City,
            GoalAmount  = dto.GoalAmount,
            DurationDays = dto.DurationDays,
            IsHidden    = dto.IsHidden,
            Status      = "Draft"
        };

        foreach (var r in dto.Rewards)
        {
            project.Rewards.Add(new Reward
            {
                Title        = r.Title,
                Description  = r.Description,
                MinAmount    = r.MinAmount,
                DeliveryMonth = r.DeliveryMonth,
                DeliveryYear  = r.DeliveryYear,
                ShippingType  = r.ShippingType,
                LimitCount    = r.LimitCount,
                SortOrder     = dto.Rewards.IndexOf(r)
            });
        }

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, new { id = project.Id });
    }

    // POST /api/projects/{id}/cover
    [Authorize]
    [HttpPost("{id:int}/cover")]
    public async Task<IActionResult> UploadCover(int id, IFormFile file)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();
        if (project.AuthorId != GetUserId()) return Forbid();

        if (file == null || file.Length == 0) return BadRequest("Файл не выбран");
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext))
            return BadRequest("Неподдерживаемый формат");
        if (file.Length > 5 * 1024 * 1024) return BadRequest("Файл слишком большой (макс. 5 МБ)");

        var dir = Path.Combine(_env.WebRootPath, "uploads", "projects", id.ToString());
        Directory.CreateDirectory(dir);
        var fileName = $"cover_{Guid.NewGuid()}{ext}";
        await using var stream = System.IO.File.Create(Path.Combine(dir, fileName));
        await file.CopyToAsync(stream);

        project.CoverImageUrl = $"/uploads/projects/{id}/{fileName}";
        project.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { url = project.CoverImageUrl });
    }

    // POST /api/projects/{id}/package
    [Authorize]
    [HttpPost("{id:int}/package")]
    public async Task<IActionResult> SelectPackage(int id, [FromBody] SelectPackageDto dto)
    {
        if (dto.PackageType != "Start" && dto.PackageType != "Advanced")
            return BadRequest("Неверный пакет");

        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();
        if (project.AuthorId != GetUserId()) return Forbid();

        project.PackageType  = dto.PackageType;
        project.PackagePaid  = true;
        project.SortPriority = dto.PackageType == "Advanced" ? 10 : 5;
        project.Status       = "Pending";
        project.UpdatedAt    = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Проект отправлен на модерацию" });
    }

    // POST /api/projects/{id}/pledge
    [Authorize]
    [HttpPost("{id:int}/pledge")]
    public async Task<IActionResult> Pledge(int id, [FromBody] CreatePledgeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId  = GetUserId();
        var user    = await _users.FindByIdAsync(userId.ToString());
        var project = await _db.Projects.FindAsync(id);

        if (project == null) return NotFound("Проект не найден");
        if (project.Status != "Active") return BadRequest("Проект не активен");
        if (user!.Balance < dto.Amount) return BadRequest(new { message = "Недостаточно средств на балансе" });

        // Reward check
        Reward? reward = null;
        if (dto.RewardId.HasValue)
        {
            reward = await _db.Rewards.FindAsync(dto.RewardId.Value);
            if (reward == null || reward.ProjectId != id) return BadRequest("Вознаграждение не найдено");
            if (dto.Amount < reward.MinAmount)
                return BadRequest($"Минимальная сумма для этого вознаграждения: {reward.MinAmount:N0} ₸");
            if (reward.LimitCount.HasValue && reward.ClaimedCount >= reward.LimitCount.Value)
                return BadRequest("Вознаграждение уже недоступно");
        }

        var balanceBefore = user.Balance;
        user.Balance -= dto.Amount;
        user.UpdatedAt = DateTime.UtcNow;

        var pledge = new Pledge
        {
            ProjectId = id, UserId = userId, RewardId = dto.RewardId,
            Amount = dto.Amount, Status = "Paid", PaidAt = DateTime.UtcNow,
            ShippingAddress = dto.ShippingAddress
        };
        _db.Pledges.Add(pledge);

        project.CollectedAmount += dto.Amount;
        project.BackersCount++;
        project.UpdatedAt = DateTime.UtcNow;

        if (reward != null) reward.ClaimedCount++;

        _db.Transactions.Add(new Transaction
        {
            UserId = userId, Type = "PledgeOut",
            Amount = dto.Amount,
            BalanceBefore = balanceBefore,
            BalanceAfter  = user.Balance,
            Note = $"Поддержка проекта «{project.Title}»"
        });

        await _db.SaveChangesAsync();

        return Ok(new PledgeResultDto
        {
            PledgeId   = pledge.Id,
            NewBalance = user.Balance,
            Message    = "Спасибо за поддержку! Ваш вклад уже помогает проекту."
        });
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

// ════════════════════════════════════════════════════════════════════
// USERS (личный кабинет)
// ════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _users;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public UsersController(AppDbContext db, UserManager<AppUser> users,
        IMapper mapper, IWebHostEnvironment env)
    {
        _db = db; _users = users; _mapper = mapper; _env = env;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var user  = await _users.GetUserAsync(User);
        if (user == null) return NotFound();
        var roles = await _users.GetRolesAsync(user);
        var dto   = _mapper.Map<UserDto>(user);
        dto.Role  = roles.FirstOrDefault() ?? "User";
        return Ok(dto);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var user = await _users.GetUserAsync(User);
        if (user == null) return NotFound();

        if (dto.FirstName  != null) user.FirstName  = dto.FirstName;
        if (dto.LastName   != null) user.LastName   = dto.LastName;
        if (dto.Phone      != null) { user.PhoneNumber = dto.Phone; user.PhoneNumberConfirmed = true; }
        if (dto.City       != null) user.City       = dto.City;
        if (dto.Address    != null) user.Address    = dto.Address;
        if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth;
        if (dto.Bio        != null) user.Bio        = dto.Bio;
        if (dto.WebSite    != null) user.WebSite    = dto.WebSite;
        user.UpdatedAt = DateTime.UtcNow;

        await _users.UpdateAsync(user);
        var roles = await _users.GetRolesAsync(user);
        var result = _mapper.Map<UserDto>(user);
        result.Role = roles.FirstOrDefault() ?? "User";
        return Ok(result);
    }

    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        var user = await _users.GetUserAsync(User);
        if (user == null) return NotFound();
        if (file == null || file.Length == 0) return BadRequest("Файл не выбран");
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext)) return BadRequest("Неверный формат");
        if (file.Length > 5 * 1024 * 1024) return BadRequest("Файл слишком большой");

        var dir = Path.Combine(_env.WebRootPath, "uploads", "avatars");
        Directory.CreateDirectory(dir);
        var fileName = $"{user.Id}_{Guid.NewGuid()}{ext}";
        await using var stream = System.IO.File.Create(Path.Combine(dir, fileName));
        await file.CopyToAsync(stream);
        user.AvatarUrl = $"/uploads/avatars/{fileName}";
        await _users.UpdateAsync(user);
        return Ok(new { url = user.AvatarUrl });
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetUserId();
        var query  = _db.Transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt);
        var total  = await query.CountAsync();
        var items  = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new PagedResult<TransactionDto>
        {
            Items = _mapper.Map<List<TransactionDto>>(items),
            TotalCount = total, Page = page, PageSize = pageSize
        });
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopUp([FromBody] TopUpDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var user = await _users.GetUserAsync(User);
        if (user == null) return NotFound();

        var before = user.Balance;
        user.Balance += dto.Amount;
        user.UpdatedAt = DateTime.UtcNow;
        await _users.UpdateAsync(user);

        _db.Transactions.Add(new Transaction
        {
            UserId = user.Id, Type = "TopUp", Amount = dto.Amount,
            BalanceBefore = before, BalanceAfter = user.Balance,
            Note = $"Пополнение баланса. Ref: {dto.PaymentRef}"
        });
        await _db.SaveChangesAsync();
        return Ok(new { newBalance = user.Balance });
    }

    [HttpGet("pledges")]
    public async Task<IActionResult> GetPledges()
    {
        var userId = GetUserId();
        var pledges = await _db.Pledges
            .Include(p => p.Project)
            .Include(p => p.Reward)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return Ok(_mapper.Map<List<PledgeDto>>(pledges));
    }

    [HttpGet("legal")]
    public async Task<IActionResult> GetLegal()
    {
        var userId = GetUserId();
        var legal  = await _db.LegalDetails.FindAsync(userId);
        if (legal == null) return Ok(null);
        return Ok(legal);
    }

    [HttpPost("legal")]
    public async Task<IActionResult> SaveLegal([FromBody] SaveLegalDto dto)
    {
        var userId = GetUserId();
        var legal  = await _db.LegalDetails.FindAsync(userId);

        if (legal == null)
        {
            legal = new LegalDetails { UserId = userId };
            _db.LegalDetails.Add(legal);
        }

        legal.LegalType  = dto.LegalType;
        legal.IIN        = dto.IIN;
        legal.BIN        = dto.BIN;
        legal.BankName   = dto.BankName;
        legal.BIK        = dto.BIK;
        legal.AccountNumber = dto.AccountNumber;
        legal.FullName   = dto.FullName;
        legal.RegAddress = dto.RegAddress;
        legal.OrgName    = dto.OrgName;
        legal.OrgLegalAddress = dto.OrgLegalAddress;
        legal.OrgFactAddress  = dto.OrgFactAddress;
        legal.OrgDirector     = dto.OrgDirector;
        legal.UpdatedAt  = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Реквизиты сохранены" });
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

// ════════════════════════════════════════════════════════════════════
// ADMIN
// ════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public AdminController(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var now = DateTime.UtcNow;
        var monthAgo = now.AddMonths(-1);
        return Ok(new AdminStatsDto
        {
            ActiveProjects  = await _db.Projects.CountAsync(p => p.Status == "Active"),
            PendingProjects = await _db.Projects.CountAsync(p => p.Status == "Pending"),
            TotalUsers      = await _db.Users.CountAsync(),
            MonthlyRevenue  = await _db.Transactions
                .Where(t => t.Type == "PledgeOut" && t.CreatedAt >= monthAgo)
                .SumAsync(t => (decimal?)t.Amount) ?? 0,
            MonthlyPledges  = await _db.Pledges
                .CountAsync(p => p.Status == "Paid" && p.CreatedAt >= monthAgo)
        });
    }

    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects([FromQuery] string? status, [FromQuery] int page = 1)
    {
        var query = _db.Projects
            .Include(p => p.Author)
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status)) query = query.Where(p => p.Status == status);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * 20).Take(20).ToListAsync();

        return Ok(new PagedResult<AdminProjectDto>
        {
            Items = _mapper.Map<List<AdminProjectDto>>(items),
            TotalCount = total, Page = page, PageSize = 20
        });
    }

    [HttpPut("projects/{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();

        project.Status    = dto.Status;
        project.AdminNote = dto.AdminNote;
        project.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == "Active")
        {
            project.StartDate = DateTime.UtcNow;
            project.EndDate   = DateTime.UtcNow.AddDays(project.DurationDays);
        }
        await _db.SaveChangesAsync();
        return Ok(new { message = "Статус обновлён" });
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1)
    {
        var total = await _db.Users.CountAsync();
        var users = await _db.Users
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * 20).Take(20)
            .Select(u => new
            {
                u.Id, u.Email, u.FirstName, u.LastName,
                u.Balance, u.EmailConfirmed, u.CreatedAt
            }).ToListAsync();
        return Ok(new { items = users, totalCount = total, page, pageSize = 20 });
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] int page = 1)
    {
        var total = await _db.Transactions.CountAsync();
        var items = await _db.Transactions
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * 20).Take(20)
            .Select(t => new
            {
                t.Id, t.Type, t.Amount, t.Note, t.CreatedAt,
                UserEmail = t.User.Email
            }).ToListAsync();
        return Ok(new { items, totalCount = total, page, pageSize = 20 });
    }
}
