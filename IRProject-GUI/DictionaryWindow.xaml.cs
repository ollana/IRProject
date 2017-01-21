using FirstFloor.ModernUI.Windows.Controls;
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
using System.ComponentModel;

namespace IRProject_GUI
{
    /// <summary>
    /// Interaction logic for DictionaryWindow.xaml
    /// </summary>
    public partial class DictionaryWindow : ModernWindow
    {
        public DictionaryWindow()
        { 
            InitializeComponent();
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
        }

        /// <summary>
        /// dictinary to display
        /// </summary>
        /// <param name="dic">dictionary</param>
        /// <param name="stemming">flag of stemming</param>
        public void TableDic(Dictionary<string,int> dic, bool stemming)
        {
            if (stemming)
                main.Title = "Dictionary With Stamming";
            else
                main.Title = "Dictionary Without Stamming";
            List<string> term = new List<string>();
            term.Add("Term  -  Frequency");
            foreach (var item in dic)
            {
                term.Add(item.Key + "  -  " + item.Value);
            }
            Term.ItemsSource = term;
        }
    }   
}
