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

            MixandPrepare(tmparray, gamewords, count);
            
        }

        public void Init(string categoryhash, MongoRepository repo, bool weak, int count = 0)
        {
            r = new Random();
            _repo = repo;
            gamewords = new List<WordModel>();
            var coll = repo.GetWeakCollection(categoryhash);

            WordModel[] tmparray = coll.ToArray();

            MixandPrepare(tmparray, gamewords, count);

        }



        public void Init(List<string> categoryhashes, MongoRepository repo, int count = 0)
        {
            r = new Random();
            _repo = repo;
            gamewords = new List<WordModel>();

            IEnumerable<WordModel> tmparray = new WordModel[0];

            foreach (string item in categoryhashes)
            {
                var coll = repo.GetCollection(item);
                tmparray = tmparray.Concat(coll.ToArray());
            }
            MixandPrepare(tmparray.ToArray(), gamewords, count);
        }

        public void Init(MongoRepository repo, bool weak, int count = 0)
        {
            r = new Random();
            _repo = repo;
            gamewords = new List<WordModel>();
            var coll = repo.GetAllCollection();
            WordModel[] tmparray = coll.ToArray();

            MixandPrepare(tmparray, gamewords, count);
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
            for (int i = 0; i < 100000; i++)
            {
                a = r.Next(0, arraytomix.Length);
                b = r.Next(0, arraytomix.Length);
                WordModel tmp = arraytomix[a];
                arraytomix[a] = arraytomix[b];
                arraytomix[b] = tmp;
            }
        }

        private void MixandPrepare(WordModel[] tmparray, List<WordModel> gamewords, int count)
        {
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

        public void AddResult(string wordhash, int result, int note, int time)
        {
            WordModel actual = _repo.GetWord(wordhash);
            actual.LastAccess = DateTime.Now;
            if (actual.ReactionTime > 0)
            {
                actual.ReactionTime = (actual.ReactionTime + time) / 2;
            }
            else
            {
                actual.ReactionTime = time;
            }
            
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
