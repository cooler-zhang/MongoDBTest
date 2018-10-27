using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest
{
    public abstract class NoSqlDBContext : IDisposable
    {
        public abstract void Dispose();
    }
}
