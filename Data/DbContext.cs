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
        var LOG_SQL = false;
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
            .Entity<SampleData>(
                eb =>
                {
                    eb.HasKey(x => x.Id);
                    eb.Property(x => x.Id).ValueGeneratedOnAdd();
                    eb.HasIndex(x => x.Name);
                    //eb.HasIndex(x => new { x.Field1, x.Field2 }).IsUnique();
                });
    }

    /// <summary>
    /// Data table
    /// </summary>    
    public DbSet<SampleData> Datas { get; set; }

}
