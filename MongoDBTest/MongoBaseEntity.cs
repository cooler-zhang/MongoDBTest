using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest
{
    public class MongoBaseEntity
    {
        public ObjectId Id { get; set; }
    }
}
