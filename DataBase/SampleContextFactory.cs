using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;

namespace DataBase
{
    internal class SampleContextFactory : IDesignTimeDbContextFactory<TwoCDbContext>
    {
        public TwoCDbContext CreateDbContext(string[] args)
        {
            return new TwoCDbContext(new DefaultConfigurationDatabase());
        }

    }
}
