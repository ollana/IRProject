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
using System.Windows.Shapes;

namespace yad2.View
{
    /// <summary>
    /// Interaction logic for Ad.xaml
    /// </summary>
    public partial class Ad : Window
    {

        Control.Controller m_c;
        
        string m_AdID;

        public Ad(Control.Controller c , string AdID)
        {
             InitializeComponent();
            m_c = c;
            m_AdID = AdID;
            Location.Text = m_c.AdLocation(Convert.ToInt32( AdID));
            Category.Text = m_c.GroupCategory(AdID);
            Manager.Text = "Manager: " + m_c.GroupManager(AdID);
            Members.ItemsSource = m_c.GroupMembers(AdID);
            About.Text = m_c.AdAbout(Convert.ToInt32(AdID));
           
        }

        private void sendRequest_Click(object sender, RoutedEventArgs e)
        {
          //  m_c.AddNewRequest(Convert.ToInt32(m_AdID), m_userMail);
          //  MessageBox.Show("Request was sent");
        }
    }
}
