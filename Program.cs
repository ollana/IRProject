﻿using System;
using System.Collections.Generic;
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
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Start testing ReadFile:");
            ReadFile rf = new ReadFile(@"D:\corpus");
            stopwatch.Stop();
            Console.WriteLine("All took " + (double)stopwatch.ElapsedMilliseconds / 60000 + " minutes");
            Console.ReadKey();
        }
    }

    public class ProgramUI
    {
        public ProgramUI() { }
        public object[] MainUI(string corpus, string destination, bool stemming)
        {
            object[] obj;
            IRSettings.Default.Corpus = corpus;
            IRSettings.Default.Destination = destination;
            IRSettings.Default.Stemming = stemming;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
           // try
           // {
                ReadFile rf = new ReadFile(IRSettings.Default.Corpus);   
                obj = new object [] {(double)stopwatch.ElapsedMilliseconds / 60000, rf.GetNumberOfDocuments() , rf.GetNumOfUniqeTerms()};
                stopwatch.Stop();
                return obj;
                //  }catch
                //  {
                //   return null;
                //  }

        }
    }
}