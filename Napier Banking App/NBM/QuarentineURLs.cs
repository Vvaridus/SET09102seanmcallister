using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NBM
{
    public static class QuarentineURLs
    {
        //search the text for URLs and return the URL(s) found to be replaced.
        static public string replaceURLWords(this string originalUrl, string replaceWithWord, string header, string sender, RegexOptions regexOptions = RegexOptions.None)
        {
            string pattern = @"(((http|ftp|https):\/\/)?[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:\/~\+#]*[\w\-\@?^=%&amp;\/~\+#])?)";
            string ret = Regex.Replace(originalUrl, pattern, replaceWithWord, regexOptions);

            //test for upload
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //search the text for URLs and write them to a Quarentine List.txt
            foreach (Match item in Regex.Matches(originalUrl, @"(((http|ftp|https):\/\/)?[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:\/~\+#]*[\w\-\@?^=%&amp;\/~\+#])?)"))
            {
                using (StreamWriter outputfile = new StreamWriter(Path.Combine(docPath, "zzQuarentine List.txt"), true))
                {
                    outputfile.WriteLine("MessageID:" + header + " Sender:" + sender + " URL:" + item + " Time and Date:" + DateTime.Now);                    
                }
            }
            
            return ret;
        }

    }
}
