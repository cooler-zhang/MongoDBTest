using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest
{
    public class MongoDbContext : NoSqlDBContext
    {
        public IMongoClient _client;
        public IMongoDatabase _database;

        public MongoDbContext(string dbName)
        {
            var setting = new MongoClientSettings();
            setting.Servers = new List<MongoServerAddress>() { new MongoServerAddress("127.0.0.1", 27017) };
            _client = new MongoClient(setting);
            _database = _client.GetDatabase(dbName);
        }

        #region Data
        public T Find<T>(string id)
            where T : MongoBaseEntity
        {
            var expression = GetCollectionExpression<T>();
            return expression.Where(a => a.Id == new ObjectId(id)).FirstOrDefault();
        }

        public IList<T> GetList<T>(Expression<Func<T, bool>> where = null)
            where T : MongoBaseEntity
        {
            var expression = GetCollectionExpression<T>();
            if (where != null)
            {
                expression = expression.Where(where);
            }
            return expression.ToList();
        }

        public IList<T> GetPageList<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, bool>> order, bool isDesc, out int totalCount)
            where T : MongoBaseEntity
        {
            var expression = GetCollectionExpression<T>();
            if (where != null)
            {
                expression = expression.Where(where);
            }
            totalCount = expression.Count();
            if (order != null)
            {
                if (isDesc)
                {
                    expression = expression.OrderByDescending(order);
                }
                else
                {
                    expression = expression.OrderBy(order);
                }
            }
            return expression.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public T GetFirst<T>(Expression<Func<T, bool>> where = null)
            where T : MongoBaseEntity
        {
            var expression = GetCollectionExpression<T>();
            if (where != null)
            {
                expression = expression.Where(where);
            }
            return expression.FirstOrDefault();
        }

        public void Create<T>(T t)
            where T : MongoBaseEntity
        {
            _database.GetCollection<T>(typeof(T).FullName).InsertOne(t);
        }

        public void Update<T>(T t)
            where T : MongoBaseEntity
        {
            var filter = MongoDB.Driver.Builders<T>.Filter.Eq("_id", t.Id);
            var update = MongoDB.Driver.Builders<T>.Update.Push("Id", t.Id);
            _database.GetCollection<T>(typeof(T).FullName).UpdateOne(filter, update);
        }

        public void Delete<T>(string id)
            where T : MongoBaseEntity
        {
            var filter = MongoDB.Driver.Builders<T>.Filter.Eq("_id", id);
            _database.GetCollection<T>(typeof(T).FullName).DeleteOne(filter);
        }
        #endregion

        #region Collection
        public void DropCollection<T>()
        {
            _database.DropCollection(typeof(T).FullName);
        }
        #endregion

        #region Index
        public string CreateIndex<T>(IndexKeysDefinition<T> keys, CreateIndexOptions options = null)
        {
            return _database.GetCollection<T>(typeof(T).FullName).Indexes.CreateOne(keys, options);
        }

        public void CreateIndexes<T>(IEnumerable<CreateIndexModel<T>> models)
        {
            _database.GetCollection<T>(typeof(T).FullName).Indexes.CreateMany(models);
        }

        public void DropIndex<T>(string name)
        {
            _database.GetCollection<T>(typeof(T).FullName).Indexes.DropOne(name);
        }

        public void DropAllIndexes<T>()
        {
            _database.GetCollection<T>(typeof(T).FullName).Indexes.DropAll();
        }
        #endregion

        #region Aggregate
        #endregion

        #region Other
        public override void Dispose()
        {
        }

        private IMongoQueryable<T> GetCollectionExpression<T>()
        {
            return _database.GetCollection<T>(typeof(T).FullName).AsQueryable<T>();
        }
        #endregion
    }
}
