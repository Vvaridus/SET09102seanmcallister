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

namespace NBM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath = @"C:\Users\Vv\Desktop\Napier Banking App\NBM\files\textwords.csv";
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        StreamReader reader = null;
        List<string> listA = new List<string>();
        List<string> listB = new List<string>();



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
        }
    }
}
