using GoldenPagesUz.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GoldenPagesUz.Data;

public class YpDbContext : DbContext
{
    private readonly IWebHostEnvironment _webHostEnvoronment;

    public YpDbContext(DbContextOptions<YpDbContext> options, IWebHostEnvironment webHostEnvoronment) : base(options)
    {
        _webHostEnvoronment = webHostEnvoronment;
    }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var path = Path.Combine(_webHostEnvoronment.ContentRootPath, "wwwroot", "categories.json");
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            var categories = JsonConvert.DeserializeObject<List<Category>>(json);

            if (categories?.Count > 0)
                modelBuilder.Entity<Category>().HasData(categories);
        }
    }
}