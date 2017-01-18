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

namespace IRProject_GUI
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : UserControl
    {
        List<string> m_langueges;
        List<string> Chosen_languages;
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
                item.Content = line;
                Language_select.Items.Add(item);
                m_langueges.Add(line);
            }
        }

        private void file_path_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void query_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Browse_file(object sender, RoutedEventArgs e)
        {

        }

        private void Browse_Dest(object sender, RoutedEventArgs e)
        {

        }

        private void auto_complete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void All_Languages_Click(object sender, RoutedEventArgs e)
        {
            if(sender is CheckBox)
            {
                if ((sender as CheckBox).IsChecked.Value)
                    Language_select.IsEnabled = false;
                else
                    Language_select.IsEnabled = true;
            }
        }

        private void languages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Save_file(object sender, RoutedEventArgs e)
        {

        }

        private void Save_path_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
