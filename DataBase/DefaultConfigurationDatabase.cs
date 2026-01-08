using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class DefaultConfigurationDatabase : IConfigurationDatabase
    {
        public string ConnectionString => "Host=localhost;Port=5432;Database=2C_cource;Username=postgres;Password=daa200513dr";
    }
}
