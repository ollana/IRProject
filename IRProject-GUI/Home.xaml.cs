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
        public Dictionary<string, Tuple<string, string>> Dictionary1; 
        public string Corpus { get { return corpus_path.Text; } set { corpus = value; corpus_path.Text = value; } }
        public string Destination { get { return save_path.Text; } set { destination = value; save_path.Text = value; } }
        public bool Stemming { get { return stemming.IsChecked.Value; } set { stemm = value; } }


        public
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
        Searcher m_searcher;

        /// <summary>
        /// constractor
        /// </summary>
        public Home()
        {
            InitializeComponent();
            Corpus = System.IO.Directory.GetCurrentDirectory();
            Destination = System.IO.Directory.GetCurrentDirectory();
            stemming.IsChecked = true;
            Stemming = true;
            no_stemming.IsChecked = false;
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            //bw.WorkerSupportsCancellation = true;
            //bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            string lineOfContents =Properties.Resources.LANGUAGES;
            string[] splitLang = lineOfContents.Split('\n');
            foreach (var line in splitLang)
            {
                comboBox.Items.Add(line);
            }

            _ui = TaskScheduler.FromCurrentSynchronizationContext();
        }
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
            IRProject.ProgramUI pui = new IRProject.ProgramUI();
            obj = pui.MainUI(corpus, destination, stemm);
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
                Dictionary1 = null;
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
            m_searcher = new Searcher(Corpus);
            Display_dic.IsEnabled = false;
            if (!System.IO.Directory.Exists(Destination))
            {
                MessageBox.Show("Destination path could not be found", "path not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string path;
            if (Stemming)
                path = Destination + "\\" + UISettings.Default.DictionaryWithStemming;
            else
                path = Destination + "\\" + UISettings.Default.DictionaryWithoutStemming;
            if (System.IO.File.Exists(path))
            {
                m_searcher.LoadDictionary(path);   
                Dictionary1 =  new Dictionary<string, Tuple<string, string>>();

                using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                {
                    string line = sr.ReadLine();
                    int lineNum = 0;
                    
                    while (line!=null)
                    {
                        if(line!=string.Empty)
                        {
                            string[] split = line.Split('|');
                            Dictionary1.Add(split[0], new Tuple<string, string>(split[1], split[2]));
                        }
                        line = sr.ReadLine();
                        lineNum++;
                    }                    
                }
                MessageBox.Show("Dictionary was loaded");
            }
            else
                MessageBox.Show("could not find dictionary path:\n" + path,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            path = Destination + "\\Pairs-WithoutStemming";

            if (System.IO.File.Exists(path))
            {

                m_searcher.LoadPairs(path);
            }
            else
                MessageBox.Show("could not find dictionary path:\n" + path, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
           //   m_searcher.Search("wildcat wildcat million wild-cat wildcat 0.1");

            Display_dic.IsEnabled = true;
        }
        /// <summary>
        /// opens a new window that displays the dictionary
        /// </summary>
        private void Display_Click(object sender, RoutedEventArgs e)
        {
            if (Dictionary1 == null)
            {
                MessageBox.Show("there is no dictionary in the program memory.\nplease select load first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                DictionaryWindow dw = new DictionaryWindow();
                dw.TableDic(Dictionary1, stemm);
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

    }
}
