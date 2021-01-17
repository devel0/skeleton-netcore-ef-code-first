using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace skeleton_netcore_ef_code_first
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder();            

            return new MyDbContext(optionsBuilder.Options);
        }
    }
}