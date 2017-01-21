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
        public string QueryFile { get { return file_path.Text; } set { _queryFile = value; file_path.Text = value; } }
        public string QuerySavePath { get { return Save_path.Text; } set { _querySavePath = value; Save_path.Text = value; m_program.SaveQuery = value; } }
        public string Query { get { return query.Text; } set { _query = value; query.Text = value;} }
        public List<string> Languages { get { return Chosen_languages; } set { Chosen_languages=value; } }
        public List<Tuple<int, string>> SearchResults { get { return results; } set { results = value; } }
        public Searcher MySearcher { get { return Home.m_searcher; } }

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
        List<string> m_langueges;
        List<string> Chosen_languages;
        List<string> autoComplete;
        List<Tuple<int,string>> results;
        int randomIndex = 451;

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
            Languages.Add("All");
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
            SearchResults = new List<Tuple<int, string>>();
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
        /// <summary>
        /// if the serch from a file iterate lines of querys and send one by one to the searcher.
        /// then if save path is not empty, save results to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SEARCH_Click(object sender, RoutedEventArgs e)
        {
            if(!Home.DicLoaded)
            {
                MessageBox.Show("Please load application files first... ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SearchResults.Clear();
            Results_lv.Visibility = Visibility.Hidden;

            bool ok = false;
            List<string> tempResults = new List<string>();
            if (from_radio.IsChecked.Value)//from file 
            {
                if (!System.IO.File.Exists(QueryFile))
                {
                    MessageBox.Show("The query file was not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                    ok = SearchFromFile();
            }
            else //from text
            {
                if (Query.Trim() == string.Empty)
                {
                    MessageBox.Show("Nothing to search..", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    tempResults = MySearcher.Search(Query, Languages);
                    AddtoResults(tempResults, randomIndex);
                    randomIndex++;
                    if (tempResults.Count == 0)
                    {
                        MessageBox.Show("There are no documents matching your query :(");
                        return;
                    }
                    ok = true;
                }
                catch
                {
                    MessageBox.Show("Cant read this kind of query :(");
                    ok = false;
                }
            }
            
            if (QuerySavePath != string.Empty && ok)
            {
                if (!System.IO.Directory.Exists(QuerySavePath))
                {
                    if (
                    MessageBox.Show("Can't find directory to save the results.. continue without saving?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error)
                    != MessageBoxResult.Yes)
                        return;
                }
                else
                {
                    saveResultsToFile();
                }
            }
            if (ok)
            {
                showResults();
                MessageBox.Show("Results are ready");
            }
        }
        /// <summary>
        /// show search results
        /// </summary>
        private void showResults()
        {
            Search_Results_num.Text = SearchResults.Count.ToString();
            Search_Results_num.Visibility = System.Windows.Visibility.Visible;
            Search_Results.Visibility = System.Windows.Visibility.Visible;
            Results_lv.ItemsSource = SearchResults;
            Results_lv.Visibility = Visibility.Visible;

        }
        /// <summary>
        /// save the qurey results to a file that traceival program can read
        /// </summary>
        private void saveResultsToFile()
        {
            string filePath = QuerySavePath + "\\results.txt";
            System.IO.File.Create(filePath).Close();
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath))
            {
                foreach (Tuple<int,string> res in SearchResults)
                {
                    sw.WriteLine(res.Item1 + " " + 0 + " " + res.Item2 + " " + 0 + " " + 0 + " mt");
                }
            }
        }
        /// <summary>
        /// shearch from file
        /// </summary>
        private bool SearchFromFile()
        {
            List<string> tempResults = new List<string>();
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(QueryFile))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        string[] splite = line.Split(' ');
                        string queryNum = splite[0].Trim();
                        string queryStirng = line.Substring(queryNum.Length).Trim();
                        int n;
                        if (int.TryParse(queryNum, out n))
                        {
                            tempResults = MySearcher.Search(queryStirng, Languages);
                            AddtoResults(tempResults, n);
                            tempResults.Clear();
                        }
                        line = sr.ReadLine();
                    }
                    return true;
                }
            }
            catch
            {
                MessageBox.Show("Cant read this kind of query file :(");
                return false;
            }
        }
        /// <summary>
        /// add searcher results to list
        /// </summary>
        /// <param name="tempResults"></param>
        /// <param name="n"></param>
        private void AddtoResults(List<string> tempResults, int n)
        {
            foreach (string doc in tempResults)
            {
                SearchResults.Add(new Tuple<int, string>(n, doc));
            }
        }
        /// <summary>
        /// select to search in all languages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// add or remove languge from choosen languages list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void languages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Languages.Clear();
            for (int i = 0; i < Language_select.SelectedItems.Count; i++)
            {
                Languages.Add(((ListBoxItem)Language_select.SelectedItems[i]).Content.ToString());
            }
        }
        /// <summary>
        /// open folder browes to choose a folder to save the query results. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_file(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            QuerySavePath = dialog.SelectedPath;
        }
        /// <summary>
        /// when entering a string to the save to file text box - save it to querySavePath
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_path_LostFocus(object sender, RoutedEventArgs e)
        {
            QuerySavePath = (sender as TextBox).Text;
        }
        /// <summary>
        /// auto complete for one term in the query serch text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void query_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (Query.Split(' ').Length == 1)
                {
                    if (MySearcher != null)
                    {
                        autoComplete = MySearcher.AutoComplete(Query);
                        if (autoComplete.Count > 0) auto_complete1.Content = autoComplete[0];
                        if (autoComplete.Count > 1) auto_complete2.Content = autoComplete[1];
                        if (autoComplete.Count > 2) auto_complete3.Content = autoComplete[2];
                        if (autoComplete.Count > 3) auto_complete4.Content = autoComplete[3];
                        if (autoComplete.Count > 4) auto_complete5.Content = autoComplete[4];
                        auto_complete.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
            else if(e.Key == Key.Back)
            {
                foreach (ListBoxItem item in auto_complete.Items)
                {
                    item.Content = "";
                }
                auto_complete.Visibility = System.Windows.Visibility.Hidden;
            }
            if (e.Key == Key.Down)
            {
                if (auto_complete.SelectedIndex < 4)
                    auto_complete.SelectedIndex++;
            }
            else if (e.Key == Key.Up)
            {
                if (auto_complete.SelectedIndex > 0)
                    auto_complete.SelectedIndex--;
            }
            else if (e.Key == Key.Tab)
            {
                Query += ((ListBoxItem)auto_complete.SelectedItem).Content.ToString();
                foreach (ListBoxItem item in auto_complete.Items)
                {
                    item.Content = "";
                }
                auto_complete.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        /// <summary>
        /// select to search a query/querys by a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void From_File_radio(object sender, RoutedEventArgs e)
        {
            brows_file_path.IsEnabled = true;
            file_path.IsEnabled = true;
            search_radio.IsChecked = false;
            query.IsEnabled = false;
            Query = "From file";
        }
        /// <summary>
        /// select to search a query by enter a string into text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_radio(object sender, RoutedEventArgs e)
        {
            from_radio.IsChecked = false;
            brows_file_path.IsEnabled = false;
            file_path.IsEnabled = false;
            query.IsEnabled = true;
            Query = "";
        }
        /// <summary>
        /// for selecting auto complete 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void auto_complete_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Down)
            {
                if (auto_complete.SelectedIndex>0)
                    auto_complete.SelectedIndex = auto_complete.SelectedIndex--;
            }
            else if (e.Key == Key.Up)
            {
                if (auto_complete.SelectedIndex < 4)
                    auto_complete.SelectedIndex = auto_complete.SelectedIndex++;
            }
            else if (e.Key == Key.Tab)
            {
                Query += ((ListBoxItem)auto_complete.SelectedItem).Content.ToString();
                auto_complete.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        /// <summary>
        /// click on the auto complete term
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void auto_complete_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Query += ((ListBoxItem)auto_complete.SelectedItem).Content.ToString();
            auto_complete.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// clear all results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            SearchResults.Clear();
            Results_lv.Visibility = Visibility.Hidden;
            Search_Results_num.Visibility = System.Windows.Visibility.Hidden;
            Search_Results.Visibility = System.Windows.Visibility.Hidden;
            if(QuerySavePath!=""  && System.IO.File.Exists(QuerySavePath + "\\results.txt"))
                System.IO.File.Delete(QuerySavePath + "\\results.txt");


        }
    }
}
