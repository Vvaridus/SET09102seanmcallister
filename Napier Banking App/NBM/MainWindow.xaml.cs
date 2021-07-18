using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace NBM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class JsonExport
    {
        public DateTimeOffset DateTime { get; set; }
        public string Header { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string MainBodyText { get; set; }
    }

    public partial class MainWindow : Window
    {
        string filePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"files\textwords.csv");
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        StreamReader reader = null;
        List<string> listA = new List<string>();
        List<string> listB = new List<string>();
        List<WordCounter> wordCounters = new List<WordCounter>();
        List<WordCounter> wordCountersUser = new List<WordCounter>();

        public MainWindow()
        {
            InitializeComponent();
            getTextWords();            
        }

        private void textBoxCharCount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void textBoxMessageBody_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxCharCount != null)
            {
                wordCount();
            }
        }

        private void textBoxHeader_TextChanged(object sender, TextChangedEventArgs e)
        {
            headerCheck();

            if (textBoxCharCount != null)
            {
                wordCount();
            }
        }

        //Check head to see if message is SMS, Email or Twitter message.
        public void headerCheck()
        {
            if (textBoxHeader.Text != "")
            {
                string tmpHeader = textBoxHeader.Text;

                if (textBoxHeader.Text != null)
                {
                    switch (Convert.ToString(tmpHeader[0]))
                    {
                        case "S":
                            availableSMS.Visibility = Visibility.Visible;
                            availableEmail.Visibility = Visibility.Hidden;
                            availableTwitter.Visibility = Visibility.Hidden;
                            textBoxSubject.Visibility = Visibility.Hidden;
                            textBlockSubject.Visibility = Visibility.Hidden;
                            textBoxMessageBody.MaxLength = 140;
                            textBoxMessageBody.Clear();
                            textBoxSubject.Clear();
                            textBoxSender.Clear();
                            break;

                        case "E":
                            availableSMS.Visibility = Visibility.Hidden;
                            availableEmail.Visibility = Visibility.Visible;
                            availableTwitter.Visibility = Visibility.Hidden;
                            textBoxSubject.Visibility = Visibility.Visible;
                            textBlockSubject.Visibility = Visibility.Visible;
                            textBoxMessageBody.MaxLength = 1028;
                            textBoxMessageBody.Clear();
                            textBoxSender.Clear();
                            break;

                        case "T":
                            availableSMS.Visibility = Visibility.Hidden;
                            availableEmail.Visibility = Visibility.Hidden;
                            availableTwitter.Visibility = Visibility.Visible;
                            textBoxSubject.Visibility = Visibility.Hidden;
                            textBlockSubject.Visibility = Visibility.Hidden;
                            textBoxMessageBody.MaxLength = 140;
                            textBoxMessageBody.Clear();
                            textBoxSubject.Clear();
                            textBoxSender.Clear();
                            break;
                        default:
                            availableSMS.Visibility = Visibility.Hidden;
                            availableEmail.Visibility = Visibility.Hidden;
                            availableTwitter.Visibility = Visibility.Hidden;
                            textBoxSubject.Visibility = Visibility.Hidden;
                            textBlockSubject.Visibility = Visibility.Hidden;
                            textBoxMessageBody.MaxLength = 1028;
                            textBoxMessageBody.Clear();
                            textBoxSubject.Clear();
                            textBoxSender.Clear();
                            break;
                    }
                }
            }
            else
            {
                availableSMS.Visibility = Visibility.Visible;
                availableEmail.Visibility = Visibility.Visible;
                availableTwitter.Visibility = Visibility.Visible;
                textBoxSubject.Visibility = Visibility.Hidden;
                textBlockSubject.Visibility = Visibility.Hidden;
                textBoxMessageBody.MaxLength = 1028;
            }

        }

        //Word count display
        public void wordCount()
        {
            textBoxCharCount.Text = Convert.ToString(textBoxMessageBody.MaxLength - textBoxMessageBody.Text.Length);
        }
        
        //Create two lists from textwords.csv
        public void getTextWords()
        {
            if (File.Exists(filePath))
            {
                reader = new StreamReader(File.OpenRead(filePath));                
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    
                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }
            }
        }

        //Test checks for URLs and text speak on Click
        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            
            //itterate through listA and replace text speak abreviations with the abriviation and the expanded text of what the abreviation means.
            for (int i = 0; i < listA.Count; i++)
            {
                textBoxMessageBody.Text = ReplaceWord.replaceTextWords(textBoxMessageBody.Text, listA[i], listA[i] + " <" + listB[i] + ">" );
            }
            //replace all URLs found in text with URL Quarentined.
            textBoxMessageBody.Text = QuarentineURLs.replaceURLWords(textBoxMessageBody.Text, " <URL Quarentined>", textBoxHeader.Text, textBoxSender.Text);
            
            isTrending();
            isUserTrending();
            createSirList();

            //export to JSON
            jsonExport(textBoxHeader.Text, textBoxSender.Text, textBoxSubject.Text, textBoxMessageBody.Text);

        }

        public static void jsonExport(string header, string sender, string subject, string mainBodyText)
        {
            var export = new JsonExport
            {
                DateTime = DateTime.Now,
                Header = header,
                Sender = sender,
                Subject = subject,
                MainBodyText = mainBodyText
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(export, options);
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outputfile = new StreamWriter(System.IO.Path.Combine(docPath, "zzJsonFileExport.json"), true))
            {
                outputfile.WriteLine(jsonString);
            }
        }

        //twitter trnding list
        public void isTrending()
        {
            var text = textBoxMessageBody.Text;
            var regex = new Regex(@"#\w+");
            var matches = regex.Matches(text);
            

            foreach (var match in matches)
            {
                WordCounter foundWord = wordCounters.Find(x => x.word == match.ToString());
                if (foundWord == null)
                {
                    wordCounters.Add(new WordCounter(match.ToString(), 1));
                }
                else
                {
                    foundWord.frequency++;
                }

                string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (StreamWriter outputfile = new StreamWriter(System.IO.Path.Combine(docPath, "zzTrendingExport.txt"), true))
                {
                    outputfile.WriteLine(match);
                    
                }
            }

            twitterTrendingCount();
        }

        //twitter trnding list
        public void isUserTrending()
        {
            var text = textBoxMessageBody.Text;
            var regex = new Regex(@"@\w+");
            var matches = regex.Matches(text);


            foreach (var match in matches)
            {
                WordCounter foundWordUser = wordCountersUser.Find(x => x.word == match.ToString());
                if (foundWordUser == null)
                {
                    wordCountersUser.Add(new WordCounter(match.ToString(), 1));
                }
                else
                {
                    foundWordUser.frequency++;
                }

                string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (StreamWriter outputfile = new StreamWriter(System.IO.Path.Combine(docPath, "zzMentionsExport.txt"), true))
                {
                    outputfile.WriteLine(match);

                }
            }

            twitterTrendingUserCount();
        }

        public void twitterTrendingCount()
        {
            listViewTrending.Items.Clear();

            foreach (WordCounter word in wordCounters)
            {
                
                String[] rowItems = new string[] { word.word, word.frequency.ToString() };
                listViewTrending.Items.Add(rowItems[0]);
                listViewTrending.Items.Add(rowItems[1]);
            }
        }
        public void twitterTrendingUserCount()
        {
            listViewUserTrending.Items.Clear();

            foreach (WordCounter word in wordCountersUser)
            {

                String[] rowItemsUser = new string[] { word.word, word.frequency.ToString() };
                listViewUserTrending.Items.Add(rowItemsUser[0]);
                listViewUserTrending.Items.Add(rowItemsUser[1]);
            }
        }

        public void createSirList()
        {
            var text = textBoxMessageBody.Text;
            var regex = new Regex(@"Nature of Incident:(?:[^\.]|\.(?=\d))*\.");
            var regex2 = new Regex(@"Sort Code:(?:[^\.]|\.(?=\d))*\.");
            var matchesSortCode = regex2.Matches(text);
            var matchesNature = regex.Matches(text);            

            foreach (var sortCode in matchesSortCode)
            {
                string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (StreamWriter outputfile = new StreamWriter(System.IO.Path.Combine(docPath, "zzSIRList.txt"), true))
                {
                    outputfile.WriteLine("\n"+sortCode);
                }
            }
            foreach (var nature in matchesNature)
            {
                string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (StreamWriter outputfile = new StreamWriter(System.IO.Path.Combine(docPath, "zzSIRList.txt"), true))
                {
                    outputfile.WriteLine(nature);
                }
            }
        }

        private void listViewTrending_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
