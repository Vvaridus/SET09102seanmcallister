using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NBM
{
    public static class ReplaceWord
    {
        //replace text words abriviations with abriviation and expanded words.
        static public string replaceTextWords(this string originalWord, string findWord, string replaceWithWord, RegexOptions regexOptions = RegexOptions.None)
        {
            string pattern = String.Format(@"\b{0}\b", findWord);
            string ret = Regex.Replace(originalWord, pattern, replaceWithWord, regexOptions);
            return ret;
        }
    }
}
