using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataBase
{
    internal class SampleContextFactory : IDesignTimeDbContextFactory<TwoCDbContext>
    {
        public TwoCDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TwoCDbContext>();

            var connectionString =
                "Host=localhost;Port=5432;Database=2C_cource;Username=postgres;Password=daa200513dr";

            optionsBuilder.UseNpgsql(connectionString, o => o.SetPostgresVersion(16, 2));

            return new TwoCDbContext(optionsBuilder.Options);
        }
    }
}
