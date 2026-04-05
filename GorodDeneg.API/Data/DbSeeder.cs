using GorodDeneg.API.Models;
using Microsoft.AspNetCore.Identity;

namespace GorodDeneg.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext db,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        // ── Roles ────────────────────────────────────────
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        // ── Admin user ───────────────────────────────────
        const string adminEmail = "admin@goroddeneg.kz";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Администратор",
                LastName = "Системы",
                PhoneNumber = "+77771234567",
                City = "Алматы",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var r = await userManager.CreateAsync(admin, "Admin@123456!");
            if (r.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── Demo user ────────────────────────────────────
        const string userEmail = "aibek@goroddeneg.kz";
        AppUser? demoUser = null;
        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            demoUser = new AppUser
            {
                UserName = userEmail,
                Email = userEmail,
                FirstName = "Айбек",
                LastName = "Джаксыбеков",
                PhoneNumber = "+77017654321",
                City = "Алматы",
                Bio = "Предприниматель и технологический энтузиаст",
                Balance = 48000m,
                EmailConfirmed = true
            };
            var r = await userManager.CreateAsync(demoUser, "User@123456!");
            if (r.Succeeded) await userManager.AddToRoleAsync(demoUser, "User");
        }
        else
        {
            demoUser = await userManager.FindByEmailAsync(userEmail);
        }

        // ── Demo Projects ────────────────────────────────
        if (!db.Projects.Any() && demoUser != null)
        {
            var projects = new[]
            {
                new Project
                {
                    AuthorId = demoUser.Id, CategoryId = 15,
                    Title = "EcoBottle — умная бутылка для воды",
                    ShortDesc = "Инновационная бутылка с датчиком качества воды и подогревом",
                    FullDesc = "Мы создаём умную бутылку для воды, которая следит за качеством воды, напоминает о гидратации и умеет подогревать воду до нужной температуры. Проект прошёл стадию прототипирования и успешно протестирован с группой из 200 пользователей.",
                    City = "Алматы", GoalAmount = 3_000_000, CollectedAmount = 1_850_000,
                    DurationDays = 60, Status = "Active", BackersCount = 142, SortPriority = 10,
                    StartDate = DateTime.UtcNow.AddDays(-20), EndDate = DateTime.UtcNow.AddDays(40)
                },
                new Project
                {
                    AuthorId = demoUser.Id, CategoryId = 10,
                    Title = "Казахский джаз-фестиваль 2025",
                    ShortDesc = "Первый международный джаз-фестиваль в Нур-Султане",
                    FullDesc = "Мы организуем уникальный музыкальный фестиваль, объединяющий лучших джазовых музыкантов Казахстана и мира. Три дня живой музыки, мастер-классов и джазовых джемов.",
                    City = "Нур-Султан", GoalAmount = 1_500_000, CollectedAmount = 920_000,
                    DurationDays = 45, Status = "Active", BackersCount = 87, SortPriority = 5,
                    StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(35)
                },
                new Project
                {
                    AuthorId = demoUser.Id, CategoryId = 11,
                    Title = "Школа программирования для детей",
                    ShortDesc = "Доступное обучение программированию для детей 8–16 лет",
                    FullDesc = "Открываем первую бесплатную школу программирования для детей в Шымкенте. Наша методика позволяет детям освоить Python, Scratch и основы веб-разработки в игровой форме.",
                    City = "Шымкент", GoalAmount = 2_500_000, CollectedAmount = 2_100_000,
                    DurationDays = 30, Status = "Active", BackersCount = 203, SortPriority = 8,
                    StartDate = DateTime.UtcNow.AddDays(-15), EndDate = DateTime.UtcNow.AddDays(15)
                },
                new Project
                {
                    AuthorId = demoUser.Id, CategoryId = 3,
                    Title = "Органическая ферма «Алтай»",
                    ShortDesc = "Первая органическая ферма в предгорьях Алатау без пестицидов",
                    FullDesc = "Создаём экологически чистую ферму в предгорьях Алатау. Полный отказ от пестицидов и ГМО, современные технологии полива и хранения урожая.",
                    City = "Алматы", GoalAmount = 2_000_000, CollectedAmount = 540_000,
                    DurationDays = 90, Status = "Active", BackersCount = 34, SortPriority = 0,
                    StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(85)
                },
                new Project
                {
                    AuthorId = demoUser.Id, CategoryId = 17,
                    Title = "Документальный фильм «Степь»",
                    ShortDesc = "Документальный фильм о культуре и природе казахской степи",
                    FullDesc = "Двухгодичная экспедиция по казахской степи. Мы снимаем фильм о жизни кочевников, традициях и уникальной природе великой степи.",
                    City = "Алматы", GoalAmount = 1_200_000, CollectedAmount = 680_000,
                    DurationDays = 60, Status = "Active", BackersCount = 56, SortPriority = 0,
                    StartDate = DateTime.UtcNow.AddDays(-8), EndDate = DateTime.UtcNow.AddDays(52)
                },
                new Project
                {
                    AuthorId = demoUser.Id, CategoryId = 5,
                    Title = "Настольная игра «Великий Шёлковый путь»",
                    ShortDesc = "Стратегическая игра о торговле на Шёлковом пути",
                    FullDesc = "Создаём уникальную настольную игру о купцах и торговых путях Средней Азии. Для 2–5 игроков, возраст 12+. Уже собрали 104% от цели!",
                    City = "Алматы", GoalAmount = 3_000_000, CollectedAmount = 3_120_000,
                    DurationDays = 15, Status = "Active", BackersCount = 187, SortPriority = 0,
                    StartDate = DateTime.UtcNow.AddDays(-12), EndDate = DateTime.UtcNow.AddDays(3)
                },
            };

            db.Projects.AddRange(projects);
            await db.SaveChangesAsync();

            // Add rewards for first project
            var firstProject = db.Projects.First();
            db.Rewards.AddRange(
                new Reward { ProjectId = firstProject.Id, Title = "Ранний сторонник", MinAmount = 5_000, Description = "Ваше имя в разделе «Благодарности» на сайте проекта.", DeliveryMonth = 12, DeliveryYear = 2025, SortOrder = 1 },
                new Reward { ProjectId = firstProject.Id, Title = "Поддержка мечты",  MinAmount = 15_000, Description = "Фирменная сувенирная продукция + персональное письмо от создателей.", DeliveryMonth = 1, DeliveryYear = 2026, SortOrder = 2 },
                new Reward { ProjectId = firstProject.Id, Title = "Первый экземпляр", MinAmount = 50_000, Description = "Получите первый выпуск продукта со скидкой 30% + доступ к закрытой бета-версии.", DeliveryMonth = 3, DeliveryYear = 2026, SortOrder = 3 }
            );
            await db.SaveChangesAsync();
        }
    }
}
