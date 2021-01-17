using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchAThing;
using static SearchAThing.UtilToolkit;

namespace skeleton_netcore_ef_code_first
{

    public class MyDbContext : DbContext
    {

        internal static LogLevel MinLogLevel = LogLevel.Information;

        static bool psql_initialized = false;
        static object lck_psql_initialized = new object();

        public Global global => Global.Instance;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configureNamedOptions = new ConfigureNamedOptions<ConsoleLoggerOptions>("", null);

            var optionsFactory = new OptionsFactory<ConsoleLoggerOptions>(
                new[] { configureNamedOptions },
                Enumerable.Empty<IPostConfigureOptions<ConsoleLoggerOptions>>());

            var optionsMonitor = new OptionsMonitor<ConsoleLoggerOptions>(
                optionsFactory,
                Enumerable.Empty<IOptionsChangeTokenSource<ConsoleLoggerOptions>>(),
                new OptionsCache<ConsoleLoggerOptions>());

            var loggerFactory = new LoggerFactory(
                new[] { new ConsoleLoggerProvider(optionsMonitor) },
                new LoggerFilterOptions
                {
                    MinLevel = MinLogLevel
                });

            optionsBuilder.UseNpgsql(global.ConnectionString)
            .EnableSensitiveDataLogging() // TODO: disable this to avoid SQL trace
            .UseLoggerFactory(loggerFactory);
        }

        public MyDbContext(DbContextOptions options) : base(options)
        {
            if (!psql_initialized)
            {
                lock (lck_psql_initialized)
                {
                    if (psql_initialized) return;

                    //this.EnableFirstLastAggregateFunctions();

                    if (Global.MainStarted)
                    {

                        // initialize or ensure some data

                    }

                    SaveChanges();

                    psql_initialized = true;
                }
            }
        }


        public void DebugChanges(bool breakIf = false)
        {
#if DEBUG
            ChangeTracker.DetectChanges();
            if (ChangeTracker.Entries().Where(r => r.State == EntityState.Added || r.State == EntityState.Modified).Count() > 0)
            {
                if (PrintChanges() && breakIf)
                {
                    Debugger.Break();
                }
                Console.WriteLine(Environment.StackTrace.ToString());
            }
#endif
        }

        bool PrintChanges()
        {
            var added_entries = new List<EntityEntry>();
            var modified_entries = new List<EntityEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added: added_entries.Add(entry); break;
                    case EntityState.Modified: modified_entries.Add(entry); break;
                }
            }

            var found_some = false;

            foreach (var entry in modified_entries)
            {
                var origValues = entry.OriginalValues;
                var record = entry.Entity as IRecordBase;
                if (record != null)
                {
                    Console.WriteLine($"==== UPDATED ENTRY [{entry.Entity.GetType().ToString()}] id:{record.id}");

                    foreach (var cur in entry.CurrentValues.Properties)
                    {
                        var origProp = entry.OriginalValues.Properties.FirstOrDefault(w => w.Name == cur.Name);
                        if (origProp != null && origProp.PropertyInfo != null)
                        {
                            var origVal = entry.OriginalValues[cur.Name];
                            var curVal = entry.CurrentValues[cur.Name];

                            if ((origVal == null ^ curVal == null) || (origVal != null && curVal != null && !origVal.Equals(curVal)))
                            {
                                Console.WriteLine($"  -- prop[{cur.Name}] changed from {JsonConvert.SerializeObject(origVal)} to {JsonConvert.SerializeObject(curVal)}");
                            }

                            found_some = true;
                        }
                    }
                }
            }

            return found_some;
        }

        void CheckValidate(IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                var validCtx = new ValidationContext(entity);
                Validator.ValidateObject(entity, validCtx);
            }
        }

        void MySaveChanges()
        {
            if (Global.MainStarted) // avoid to process these if in migrations
            {
                var added_entries = new List<EntityEntry>();
                var modified_entries = new List<EntityEntry>();
                foreach (var entry in ChangeTracker.Entries())
                {
                    switch (entry.State)
                    {
                        case EntityState.Added: added_entries.Add(entry); break;
                        case EntityState.Modified: modified_entries.Add(entry); break;
                    }
                }

                CheckValidate(added_entries);
                CheckValidate(modified_entries);

                foreach (var entry in modified_entries)
                {
                    var record = entry.Entity as IRecord;

                    // EP: TRIGGER UPDATE
                    record.update_timestamp = DateTime.UtcNow;
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            MySaveChanges();

            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            MySaveChanges();

            return base.SaveChanges();
        }

        /// <summary>
        /// unique index, index, delete cascade, default values required db fields 
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //
            // MAPPING ( **note** : ORDER ENTITY DECLARATIONS )
            //            

            builder.Entity<MigrationsBackup>();
            builder.Entity<SampleTable>();

            //
            // INDEX
            //                        
            //builder.Entity<SampleTable>().HasIndex(x => new { x.field });            

            //
            // UNIQUE INDEX
            //
            //builder.Entity<SampleTable>().HasIndex("id_a", "b").IsUnique();     

            //
            // INDEX through entity types
            //
            {
                foreach (var ent in builder.Model.GetEntityTypes())
                {
                    var entName = ent.Name;
                    var entType = ent.ClrType;

                    Console.WriteLine($"=== ENT NAME [{entName}]");

                    if (typeof(IRecordBase).IsAssignableFrom(entType))
                    {
                        builder.Entity(entType).HasIndex(GetMemberNames<IRecordBase>(x => new { x.update_timestamp }).ToArray());
                    }
                }
            }

            //
            // DELETE BEHAVIOR
            //
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict; //.Cascade
            }

            //
            // DEFAULT VALUES
            //            
            // builder.Entity<SampleTable>().Property(p => p.a).HasDefaultValue(A_DEFAULT);            
        }

        public DbSet<SampleTable> SampleTables { get; set; }

    }

}