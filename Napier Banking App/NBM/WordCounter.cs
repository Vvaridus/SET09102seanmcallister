using System;
using System.Collections.Generic;
using System.Text;

namespace NBM
{
    //Twitter #Hastags and @Mentions to count unique Tags and Mentions
    public class WordCounter
    {
        public string word { get; set; }
        public int frequency { get; set; }

        public WordCounter(string word, int frequency)
        {
            this.word = word;
            this.frequency = frequency;
        }
    }
}
