using LearnWords.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnWords.Data
{
    public class GameLogic
    {
        List<WordModel> gamewords;
        Random r;
        MongoRepository _repo;

        public void Init(string categoryhash, MongoRepository repo, int count = 0)
        {
            r = new Random();
            _repo = repo;
            gamewords = new List<WordModel>();
            var coll = repo.GetCollection(categoryhash);
            WordModel[] tmparray = coll.ToArray();
            RandomSort(tmparray);
            if (count > tmparray.Length || count == 0)
            {
                count = tmparray.Length;
            }

            for (int i = 0; i < count; i++)
            {
                gamewords.Add(tmparray[i]);
            }
        }

        public void Init(List<string> categoryhashes, MongoRepository repo, int count = 0)
        {
            r = new Random();
            gamewords = new List<WordModel>();
        }

        public WordModel GetNextWord()
        {
            WordModel tmp = gamewords.FirstOrDefault();
            if (tmp == null)
            {
                return null;
            }
            gamewords.Remove(tmp);
            return tmp;
        }

        private void RandomSort(WordModel [] arraytomix)
        {
            int a = 0;
            int b = 0;
            for (int i = 0; i < 10000; i++)
            {
                a = r.Next(0, arraytomix.Length);
                b = r.Next(0, arraytomix.Length);
                WordModel tmp = arraytomix[a];
                arraytomix[a] = arraytomix[b];
                arraytomix[b] = tmp;
            }
        }

        public void AddResult(string wordhash, int result, int note)
        {
            WordModel actual = _repo.GetWord(wordhash);
            actual.LastAccess = DateTime.Now;
            if (result == 1)
            {
                actual.Goods++;
            }
            else
            {
                actual.Bads++;
            }
            actual.Note = note;
            _repo.EditWord(actual);
        }
    }
}
