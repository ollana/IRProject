using IRProject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace IRProject_GUI
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : UserControl
    {
        public Dictionary<string, Tuple<string, string>> Dictionary1;
        public string QueryFile { get { return file_path.Text; } set { _queryFile = value; file_path.Text = value; } }
        public string QuerySavePath { get { return Save_path.Text; } set { _querySavePath = value; Save_path.Text = value; m_program.SaveQuery = value; } }
        public string Query { get { return query.Text; } set { _query = value; query.Text = value;} }
        public List<string> Languages { get { return Chosen_languages; } set { Chosen_languages=value; } }
        //for stop watch
        //DispatcherTimer dt = new DispatcherTimer();
        //Stopwatch stopWatch = new Stopwatch();
        //string currentTime = string.Empty;
        ////this is for the timer in the backround
        //BackgroundWorker bw = new BackgroundWorker();
        string _queryFile;
        string _query;
        string _querySavePath;
        //object[] obj;
        //TaskScheduler _ui;
        IRProject.ProgramUI m_program;
        Searcher m_searcher;
        List<string> m_langueges;
        List<string> Chosen_languages;
        List<string> autoComplete;

        /// <summary>
        /// constractor
        /// </summary>
        public Search()
        {
            InitializeComponent();
            string lineOfContents = Properties.Resources.LANGUAGES;
            string[] splitLang = lineOfContents.Split('\n');
            m_langueges = new List<string>();
            Chosen_languages = new List<string>();
            foreach (var line in splitLang)
            {
                ListBoxItem item= new ListBoxItem();
                item.Width = 100;
                item.Content = line.Trim();
                Language_select.Items.Add(item);
                m_langueges.Add(line);
            }
            All_Languages.IsChecked = true;
            Language_select.IsEnabled = false;
            m_program = new ProgramUI();
            search_radio.IsChecked = true;
            from_radio.IsChecked = false;
            brows_file_path.IsEnabled = false;
            file_path.IsEnabled = false;
            query.IsEnabled = true;
            Query = "";
        }

        /// <summary>
        /// change query file path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void file_path_LostFocus(object sender, TextChangedEventArgs e)
        {
            QueryFile = (sender as TextBox).Text;
        }

        /// <summary>
        /// change query 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void query_LostFocus(object sender, RoutedEventArgs e)
        {
            Query = (sender as TextBox).Text;
        }

        /// <summary>
        /// brows query file from dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browse_file(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            QueryFile = dialog.FileName;
        }

        private void SEARCH_Click(object sender, RoutedEventArgs e)
        {
            //בדיקות קלט
            if (from_radio.IsChecked.Value)
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(QueryFile))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        List<string> all = new List<string>();
                        all.Add("All");
                        if (line != string.Empty)
                        {
                            string[] split = line.Split(' ');
                            string queryNum = split[0];
                            int n;
                            if (int.TryParse(queryNum, out n))
                                m_searcher.Search(line.Replace(queryNum,"").Trim().ToLower(), all, Convert.ToInt32(queryNum));
                        }
                    }
                }
            }
            else
            {
                List<string> all = new List<string>();
                all.Add("All");
                m_searcher.Search(Query.ToLower(), all, 10);

            }
        }

        private void auto_complete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ListBoxItem)
            {
                Query += (sender as ListBoxItem).Content;
                auto_complete.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        private void All_Languages_Click(object sender, RoutedEventArgs e)
        {
            Languages.Clear();
            if ((sender as CheckBox).IsChecked.Value)
            {
                Language_select.IsEnabled = false;
                Languages.Add("All");
            }
            else
            {
                Language_select.IsEnabled = true;
                for (int i = 0; i < Language_select.SelectedItems.Count; i++)
                {
                    Languages.Add(((ListBoxItem)Language_select.SelectedItems[i]).Content.ToString());
                }
            }
        }

        private void languages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Languages.Clear();
            for (int i = 0; i < Language_select.SelectedItems.Count; i++)
            {
                Languages.Add(((ListBoxItem)Language_select.SelectedItems[i]).Content.ToString());
            }
        }

        private void Save_file(object sender, RoutedEventArgs e)
        {

        }

        private void Save_path_LostFocus(object sender, RoutedEventArgs e)
        {
            QuerySavePath = (sender as TextBox).Text;
        }

        private void query_KeyDown(object sender, KeyEventArgs e)
        {
            if(sender is TextBox)
            {
                if(e.Key == Key.Space)
                {
                    if (Query.Split(' ').Length == 1)
                    {
                        m_searcher = Home.m_searcher;
                        if (m_searcher != null)
                        {
                            autoComplete = m_searcher.AutoComplete(Query);
                            if (autoComplete.Count > 0) auto_complete1.Content = autoComplete[0];
                            if (autoComplete.Count > 1) auto_complete2.Content = autoComplete[1];
                            if (autoComplete.Count > 2) auto_complete1.Content = autoComplete[2];
                            if (autoComplete.Count > 3) auto_complete2.Content = autoComplete[3];
                            if (autoComplete.Count > 4) auto_complete1.Content = autoComplete[4];
                            auto_complete.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                }
            }
        }

        private void From_File_radio(object sender, RoutedEventArgs e)
        {
            brows_file_path.IsEnabled = true;
            file_path.IsEnabled = true;
            search_radio.IsChecked = false;
            query.IsEnabled = false;
            Query = "From file";
        }

        private void Search_radio(object sender, RoutedEventArgs e)
        {
            from_radio.IsChecked = false;
            brows_file_path.IsEnabled = false;
            file_path.IsEnabled = false;
            query.IsEnabled = true;
            Query = "";
        }
    }
}
