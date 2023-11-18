using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        /*
        Follow is necessary to avoid compiler error in one-to-one relationship:
        "The dependent side could not be determined for the one-to-one relationship between 'TableA.BObject' and 'TableB.AObject'. To identify the dependent side of the relationship, configure the foreign key property. If these navigations should not be part of the same relationship, configure them independently via separate method chains in 'OnModelCreating'. See http://go.microsoft.com/fwlink/?LinkId=724062 for more details"
        */
        // modelBuilder.Entity<TableB>()
        //     .HasOne(e => e.AObject)
        //     .WithOne(e => e.BObject)
        //     .HasForeignKey<TableA>(e => e.BObjectId)
        //     .IsRequired();
    }

    //

    public DbSet<TableA_One> ARecords { get; set; }

    public DbSet<TableB_One> BRecords { get; set; }

    //

    public DbSet<TableC_Many> CRecords { get; set; }

    public DbSet<TableD_One> DRecords { get; set; }

    //

    public DbSet<TableE_Many> ERecords { get; set; }

    public DbSet<TableF_Many> FRecords { get; set; }

}
