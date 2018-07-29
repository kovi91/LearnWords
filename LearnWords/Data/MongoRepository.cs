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
        IMongoCollection<WordModel> _collection;
        IMongoCollection<CategoryModel> _categories;
        Mongo _mongo;
        public MongoRepository(IConfiguration Configuration)
        {
            _mongo = new Mongo(Configuration.GetConnectionString("MongoConnection"));
            _mongo.Connect();
        }

        public void Init(string db)
        {
            _db = _mongo.GetDatabase(db);
            _collection = _db.GetCollection<WordModel>("collection");
            _categories = _db.GetCollection<CategoryModel>("categories");
        }

        public IEnumerable<CategoryModel> GetCategories()
        {
            var q = from x in _categories.Linq()
                    select x;
            return q;
            
        }

        public IEnumerable<WordModel> GetCollection(string categoryhash)
        {
            var coll = from x in _collection.Linq()
                       where x.Category == categoryhash
                       select x;
            return coll;
        }

        public void AddCategory(CategoryModel category)
        {
            _categories.Save(category);
        }

        public void AddWord(WordModel word)
        {
            _collection.Save(word);
        }

        public void DeleteWord(string wordhash)
        {
            var q = from x in _collection.Linq()
                    where x.WordHash == wordhash
                    select x;
            var wordtodelete = q.FirstOrDefault();
            if (wordtodelete != null)
            {
                _collection.Remove(wordtodelete);
            }
        }

        public WordModel GetWord(string wordhash)
        {
            var q = from x in _collection.Linq()
                    where x.WordHash == wordhash
                    select x;
            var wordtoedit = q.FirstOrDefault();
            if (wordtoedit != null)
            {
                return wordtoedit;
            }
            else
            {
                throw new Exception("Not found");
            }
        }

        public void EditWord(WordModel word)
        {
            var q = from x in _collection.Linq()
                    where x.WordHash == word.WordHash
                    select x;
            var wordtodelete = q.FirstOrDefault();
            if (wordtodelete != null)
            {
                _collection.Remove(wordtodelete);
            }
            _collection.Save(word);
        }
    }
}
