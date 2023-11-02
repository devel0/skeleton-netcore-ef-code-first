using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace skeleton_netcore_ef_code_first;

public class LocalDbContext : DbContext
{

    public bool ReadOnlyMode { get; private set; }

    public string SqlitePathfilename =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample.db");

    /// <summary>
    /// Initialize db context
    /// </summary>
    /// <param name="readonlyMode">If true db will opened in readonly mode ( note : not supported on all db types )</param>
    public LocalDbContext(bool readonlyMode = false)
    {
        ReadOnlyMode = readonlyMode;
    }

    /// <summary>
    /// configure 
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // allow to analyze linq to sql translation
        var LOG_SQL = true;
        if (LOG_SQL)
        {
            optionsBuilder.EnableSensitiveDataLogging();

            optionsBuilder.LogTo((str) =>
            {
                Console.WriteLine($"(local) DB> {str}");
            }, LogLevel.Information);
        }

        // set db backend
        optionsBuilder.UseSqlite($@"Data Source={SqlitePathfilename}{(ReadOnlyMode ? ";Mode=ReadOnly" : "")}");
    }

    /// <summary>
    /// invoked when db model created, here fluent used to configure field types, indexes, initial data seeding, etc.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Post>(entity =>
            {
                entity.HasKey(x => x.Id);                    
            })

            .Entity<Tag>(entity =>
            {
                entity.HasKey(x => x.Id);                    
            })

            .Entity<PostTag>(entity =>
            {
                entity.HasKey(x => x.Id);                    
            })
            ;
    }
    
    public DbSet<Post> Posts { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PostTag> PostTags { get; set; }

}
