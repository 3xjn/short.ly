using Microsoft.EntityFrameworkCore;
using Shortly_API.Models;

namespace Shortly_API.Models;

public class CodeMapContext : DbContext
{
    public CodeMapContext(DbContextOptions<CodeMapContext> options)
        : base(options)
    {
    }

    public DbSet<CodeMap> CodeMaps { get; set; } = null!;
}