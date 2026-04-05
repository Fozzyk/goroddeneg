namespace GorodDeneg.API.Helpers;

/// <summary>
/// Хелпер для работы с загружаемыми файлами
/// </summary>
public static class FileHelper
{
    private static readonly string[] AllowedImageTypes = { ".jpg", ".jpeg", ".png", ".webp" };
    private static readonly string[] AllowedVideoTypes = { ".mp4", ".mov", ".avi", ".webm" };
    private const long MaxImageBytes = 5  * 1024 * 1024;  // 5 MB
    private const long MaxVideoBytes = 25 * 1024 * 1024;  // 25 MB

    public static (bool isValid, string error) ValidateImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return (false, "Файл не выбран");

        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!AllowedImageTypes.Contains(ext))
            return (false, $"Неподдерживаемый формат. Разрешены: {string.Join(", ", AllowedImageTypes)}");

        if (file.Length > MaxImageBytes)
            return (false, "Файл слишком большой. Максимум 5 МБ");

        return (true, string.Empty);
    }

    public static (bool isValid, string error) ValidateVideo(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return (false, "Файл не выбран");

        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!AllowedVideoTypes.Contains(ext))
            return (false, $"Неподдерживаемый формат. Разрешены: {string.Join(", ", AllowedVideoTypes)}");

        if (file.Length > MaxVideoBytes)
            return (false, "Файл слишком большой. Максимум 25 МБ");

        return (true, string.Empty);
    }

    public static async Task<string> SaveFileAsync(
        IFormFile file,
        string webRootPath,
        string subDirectory,
        string? customFileName = null)
    {
        var ext      = Path.GetExtension(file.FileName).ToLower();
        var fileName = customFileName ?? $"{Guid.NewGuid()}{ext}";
        var dir      = Path.Combine(webRootPath, "uploads", subDirectory);

        Directory.CreateDirectory(dir);

        var fullPath = Path.Combine(dir, fileName);
        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);

        // Return web-accessible URL
        return $"/uploads/{subDirectory}/{fileName}".Replace('\\', '/');
    }

    public static void DeleteFile(string webRootPath, string relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl)) return;
        var fullPath = Path.Combine(webRootPath, relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
}

/// <summary>
/// Расширения для построения постраничных запросов
/// </summary>
public static class QueryableExtensions
{
    public static async Task<(List<T> Items, int TotalCount)> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var total = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
            .CountAsync(query, cancellationToken);

        var items = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
            .ToListAsync(
                query.Skip((page - 1) * pageSize).Take(pageSize),
                cancellationToken);

        return (items, total);
    }
}
