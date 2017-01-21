using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using IRProject;

namespace IRProject_GUI
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public string Corpus { get { return corpus_path.Text; } set { corpus = value; corpus_path.Text = value; m_program.CorpusDestination = value;} }
        public string Destination { get { return save_path.Text; } set { destination = value; save_path.Text = value; m_program.DictionaryDestination = value; } }
        public bool Stemming { get { return stemming.IsChecked.Value; } set { stemm = value; m_program.Stemming = value; } }
        static public bool DicLoaded { get { return isLoaded; } set { isLoaded = value; } }
        //for stop watch
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        string currentTime = string.Empty;
        //this is for the timer in the backround
        BackgroundWorker bw = new BackgroundWorker();
        string corpus;
        string destination;
        bool stemm;
        object[] obj;
        TaskScheduler _ui;
        static IRProject.ProgramUI m_program;
        List<string> m_langueges;
        public static Searcher m_searcher;
        static bool isLoaded;
        /// <summary>
        /// constractor
        /// </summary>
        public Home()
        {
            InitializeComponent();
            m_program= new IRProject.ProgramUI();
            Corpus = @"d:\corpus";
            Destination = @"d:\OLLADI";
            stemming.IsChecked = true;
            Stemming = true;
            DicLoaded = false;
            no_stemming.IsChecked = false;
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            //bw.WorkerSupportsCancellation = true;
            //bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            string lineOfContents =Properties.Resources.LANGUAGES;
            string[] splitLang = lineOfContents.Split('\n');
            m_langueges = new List<string>();
            foreach (var line in splitLang)
            {
                m_langueges.Add(line);
            }

            _ui = TaskScheduler.FromCurrentSynchronizationContext();
        }

        public Home(bool i){}
        /// <summary>
        /// browse for corpus directory
        /// </summary>
        private void Browse_Corpuse(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            Corpus = dialog.SelectedPath;
        }
        /// <summary>
        /// browse for directory to save files 
        /// </summary>
        private void Browse_Dest(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            Destination = dialog.SelectedPath;
        }
        /// <summary>
        /// checkbox stemming
        /// </summary>
        private void stemming_Click(object sender, RoutedEventArgs e)
        {
            if (stemming.IsChecked.Value == true)
            {
                no_stemming.IsChecked = false;
                Stemming = true;
            }
            else
            {
                no_stemming.IsChecked = true;
                Stemming = false;
            }
        }
        /// <summary>
        /// checkbox no stemming
        /// </summary>
        private void nostemming_Click(object sender, RoutedEventArgs e)
        {
            if (no_stemming.IsChecked.Value == true)
            {
                stemming.IsChecked = false;
                Stemming = false;
            }
            else
            {
                stemming.IsChecked = true;
                Stemming = true;
            }
        }

        /// <summary>
        /// for the timer to run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            obj = m_program.MainUI();

        }

        /// <summary>
        /// what to do when the work of the woker finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Error == null))
            {
                Time.Visibility = Visibility.Hidden;
                if (stopWatch.IsRunning)
                    stopWatch.Stop();
                stopWatch.Reset();
                dt.Stop();
                ClockTextBlock.Visibility = Visibility.Hidden;
                start_button.IsEnabled = true;
                reset_button.IsEnabled = true;
                MessageBox.Show("Error: " + e.Error.Message);
            }

            else
            {
                ClockTextBlock.Visibility = Visibility.Hidden;
                start_button.IsEnabled = true;
                reset_button.IsEnabled = true;
                if (stopWatch.IsRunning)
                    stopWatch.Stop();
                stopWatch.Reset();
                dt.Stop();
                if (obj != null)
                {
                    Time.Text = string.Format("Total time: {0} minutes,\nNumber of documents: {1},\nNumber of uniqe terms: {2}", ((double)obj[0]).ToString().Substring(0, 4), obj[1], obj[2]);
                    MessageBox.Show(Time.Text);
                    Load_Click(this, new RoutedEventArgs());
                }
                else
                {
                    Time.Visibility = Visibility.Hidden;
                    MessageBox.Show("Error: can't read this kind of corpus");
                }
            }
        }

        /// <summary>
        /// makes sure that the source and destination path exists, if not show error in messagebox.
        /// start RIProject with the given settings. with the help of backround worker - bw
        /// </summary>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            string PathesExist = CheckPaths();
            if (PathesExist == null) //pathes are ok
            {
                Time.Text = "Please be petient... it may take some time...";
                Time.Visibility = Visibility.Visible;
                ClockTextBlock.Visibility = Visibility.Visible;
                stopWatch.Start();
                dt.Start();
                if (bw.IsBusy != true)
                {
                    start_button.IsEnabled = false;
                    reset_button.IsEnabled = false;
                    bw.RunWorkerAsync();
                }
            }
            else
                MessageBox.Show(PathesExist, "path not found", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string CheckPaths()
        {
            if (!System.IO.Directory.Exists(Corpus))
                return "Corpus path could not be found";
            if (!System.IO.Directory.Exists(Destination))
                return "Destination path could not be found";
            return null;
        }
        /// <summary>
        /// reset main memory and deletes all conected files.
        /// </summary>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you whant to reset settings? \nthe following will delete posting and dictionary files! \nallso it will reset the main memory of the program.", "RESET", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result.ToString() == "Yes")
            {
                isLoaded = false;
                m_searcher.ResetDictionaries();
                if (!System.IO.Directory.Exists(Destination))
                {
                    MessageBox.Show("Destination path could not be found", "path not found", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string[] paths = {UISettings.Default.PostingWithoutStemming,
                                  UISettings.Default.PostingWithStemming,
                                  UISettings.Default.DictionaryWithoutStemming,
                                  UISettings.Default.DictionaryWithStemming,
                                  UISettings.Default.DocumentsWithStemming,
                                  UISettings.Default.DocumentsWithoutStemming};

                foreach (var path in paths)
                {
                    if (System.IO.File.Exists(Destination +"\\"+ path))
                        System.IO.File.Delete(Destination + "\\" + path);
                }

                Time.Visibility = Visibility.Hidden;
            }
            else return;
        }
        /// <summary>
        /// loads the dictionary to main memory
        /// </summary>
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            m_searcher = new Searcher();
            Display_dic.IsEnabled = false;
            if (!System.IO.Directory.Exists(Destination))
            {
                MessageBox.Show("Destination path could not be found", "path not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (LoadDictionaries())
            {
                Display_dic.IsEnabled = true;
                DicLoaded = true;
                MessageBox.Show("Dictionary was loaded");
            }

        }

        

        /// <summary>
        /// opens a new window that displays the dictionary
        /// </summary>
        private void Display_Click(object sender, RoutedEventArgs e)
        {
            if (!DicLoaded)
            {
                MessageBox.Show("there is no dictionary in the program memory.\nplease select load first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                DictionaryWindow dw = new DictionaryWindow();
                dw.TableDic(m_searcher.getDictionary(), stemm);
                dw.ShowDialog();
            }
            catch
            {

                MessageBox.Show("could not display dictionary...\nplease try to load the dictionary and try again.");
            }
        }
        /// <summary>
        /// method for the running stopwatch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                ClockTextBlock.Text = currentTime;
            }
        }

        /// <summary>
        /// save the path
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void corpus_path_LostFocus(object sender, RoutedEventArgs e)
        {
            Corpus = (sender as TextBox).Text;
        }

        /// <summary>
        /// save the path
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void save_path_LostFocus(object sender, RoutedEventArgs e)
        {
            Destination = (sender as TextBox).Text;

        }

        /// <summary>
        /// load dictionaries
        /// </summary>
        /// <returns>true if completed </returns>
        private bool LoadDictionaries()
        {

            string dictionaryPath, pairsFilePath, documentsDataPath, postingPath,stopWordsPath;
            if (Stemming)
            {
                dictionaryPath = Destination + "\\" + UISettings.Default.DictionaryWithStemming;
                documentsDataPath = Destination + "\\" + UISettings.Default.DocumentsWithStemming;
                postingPath = Destination + "\\" + UISettings.Default.PostingWithStemming;
            }
            else
            {
                dictionaryPath = Destination + "\\" + UISettings.Default.DictionaryWithoutStemming;
                documentsDataPath = Destination + "\\" + UISettings.Default.DocumentsWithoutStemming;
                postingPath = Destination + "\\" + UISettings.Default.PostingWithoutStemming;

            }
            pairsFilePath = Destination + "\\Pairs-WithoutStemming";
            stopWordsPath = Destination + "\\" + "stop_words.txt";
            if (System.IO.File.Exists(dictionaryPath) && System.IO.File.Exists(pairsFilePath) && System.IO.File.Exists(documentsDataPath) && System.IO.File.Exists(postingPath))
            {
                try
                {
                    m_searcher.LoadDictionaries(dictionaryPath, pairsFilePath, documentsDataPath, postingPath, stopWordsPath, m_langueges);
                    return true;
                }
                catch
                {
                    MessageBox.Show("Somthing went wrong, one or more files is defective", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
            }
            if (!System.IO.File.Exists(dictionaryPath))
                MessageBox.Show("could not find dictionary in path:\n" + dictionaryPath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            if (!System.IO.File.Exists(pairsFilePath))
                MessageBox.Show("could not find Pairs file in path:\n" + pairsFilePath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            if (!System.IO.File.Exists(documentsDataPath))
                MessageBox.Show("could not find Documents file in path:\n" + dictionaryPath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            if (!System.IO.File.Exists(postingPath))
                MessageBox.Show("could not find Posting file in path:\n" + dictionaryPath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

    }
}
