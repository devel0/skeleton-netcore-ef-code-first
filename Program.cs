using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace skeleton_netcore_ef_code_first
{
    class Program
    {
        static void Main(string[] args)
        {
            MyDbContext.MinLogLevel = LogLevel.Information;

            var global = Global.Instance;

            var applicationDbContextFactory = new AppDbContextFactory();

            using (var ctx = applicationDbContextFactory.CreateDbContext(args))
            {
                MigrationsTools.CheckMigrationsToolCmdline(args);

                Global.MainStarted = true;

                var q = ctx.SampleTables.OrderByDescending(w => w.create_timestamp).FirstOrDefault();

                int nextval = 0;
                if (q != null)
                {
                    System.Console.WriteLine($"last created record @[{q.create_timestamp}] some_data={q.some_data}");
                    nextval = q.some_data + 1;
                }

                var newRecord = new SampleTable
                {                    
                    create_timestamp = DateTime.UtcNow,
                    some_data = nextval
                };
                ctx.SampleTables.Add(newRecord);

                ctx.SaveChanges();

                System.Console.WriteLine($"new record added : " + JsonConvert.SerializeObject(newRecord, Formatting.Indented));
            }

        }
    }
}
