using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        public IEnumerable<WordModel> GetAllCollection()
        {
            var coll = from x in _collection.Linq()
                       select x;
            return coll;
        }

        public void AddCategory(CategoryModel category)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, category.Name + category.Description);
                category.Hash = hash;
            }

            var q = from x in _categories.Linq()
                    select x;

            int count = q.ToArray().Count();
            category.Id = count;

            _categories.Save(category);
        }

        public async Task<string> AddWord(WordModel word)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, word.HomeLang+word.ForeLang);
                word.WordHash = hash;
            }

            if (word.Picture == null || word.Picture.Length == 0)
            {
                word.ImageUrl = "none.JPG";
            }
            else
            {
                var ff = word.Picture;

                var filePath = Path.GetTempFileName();

                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/images/wordpictures",
                            word.WordHash + "." + ff.FileName.Split('.')[1]);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ff.CopyToAsync(stream);
                }

                word.ImageUrl = word.WordHash + "." + ff.FileName.Split('.')[1];
            }
            word.Picture = null;
            _collection.Save(word);
            return "alma";
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
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
