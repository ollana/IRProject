using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRProject
{
    class Program
    {
        /// <summary>
        /// testtttttttttt
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();
            //Console.WriteLine("Start testing ReadFile:");
            //ReadFile rf = new ReadFile(@"D:\corpus");
            //stopwatch.Stop();
            //Console.WriteLine("All took " + (double)stopwatch.ElapsedMilliseconds / 60000 + " minutes");
            //Console.ReadKey();
            string path = @"d:\\olladi\\a.txt";
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = File.ReadLines(path).Skip(14).Take(1).First();

                }
            }
        }
    }

    public class ProgramUI
    {
        public ProgramUI() { }
        public object[] MainUI()
        {
            object[] obj;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
           // try
           // {
                ReadFile rf = new ReadFile();   
                obj = new object [] {(double)stopwatch.ElapsedMilliseconds / 60000, rf.GetNumberOfDocuments() , rf.GetNumOfUniqeTerms()};
                stopwatch.Stop();
                return obj;
                //  }catch
                //  {
                //   return null;
                //  }

        }

        public bool Stemming
        {
            set { IRSettings.Default.Stemming = value; }

        }
        public string CorpusDestination
        {
            set { IRSettings.Default.Corpus = value; }

        }
        public string DictionaryDestination
        {
            set { IRSettings.Default.Destination = value; }
        }

        public string SaveQuery
        {
            set { IRSettings.Default.SaveQuery = value; }
        }
    }
}