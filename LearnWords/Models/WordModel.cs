using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnWords.Models
{
    public class WordModel
    {
        public string WordHash { get; set; }

        public string HomeLang { get; set;}

        public string ForeLang { get; set;}

        public string Category { get; set;}

        public DateTime LastAccess { get; set; }

        public int Goods { get; set; }

        public int Bads { get; set; }

        public int Note { get; set; }

        public IFormFile Picture { get; set; }

        public string ImageUrl { get; set; }

        public double ReactionTime { get; set; }

        public string Description { get; set; }

    }
}
