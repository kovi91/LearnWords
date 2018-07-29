using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnWords.Models;
using Microsoft.Extensions.Configuration;
using MongoDB;
using MongoDB.Linq;

namespace LearnWords.Data
{
    public class MongoRepository
    {
        IMongoDatabase _db;
        IMongoCollection _collection;
        Mongo _mongo;
        public MongoRepository(IConfiguration Configuration)
        {
            _mongo = new Mongo(Configuration.GetConnectionString("MongoConnection"));
            _mongo.Connect();
        }

        public void Change(string db, string collection)
        {
            _db = _mongo.GetDatabase(db);
            var _collection = _db.GetCollection<WordModel>(collection);
        }

        public IEnumerable<CategoryModel> GetCategories()
        {
            var _collection = _db.GetCollection<CategoryModel>("categories");
            var q = from x in _collection.Linq()
                    select x;
            return q;
            
        }

        public void AddCategory(CategoryModel category)
        {
            var _collection = _db.GetCollection<CategoryModel>("categories");
            _collection.Save(category);
        }
    }
}
