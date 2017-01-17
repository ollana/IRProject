
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IRProject
{
    /// <summary>
    /// this method gets a list of terms and makes an inventered index 
    /// </summary>
    class Indexer
    {
        int FileCount;
        int numOfUniqueTerms;
        /// <summary>
        /// number of unique terms
        /// </summary>
        public int UniqueTerms
        {
            get { return numOfUniqueTerms; }
        }
        /// <summary>
        /// Empty constractor
        /// </summary>
        public Indexer()
        {
            FileCount = 0;
            numOfUniqueTerms = 0;
        }
        /// <summary>
        /// this method gets a list of unsorted terms, sorts them and group all identical terms (with the same value). 
        /// </summary>
        /// <param name="TermsList"> term list</param>
        ///<param Pairs="terms"> list of pairs</param>
        public void Index(List<ParsedTerm> TermsList, List<string> Pairs)
        {
            ParsedTerm[] TermsArray = TermsList.OrderBy(t => t.Value, StringComparer.Ordinal).ToArray();
            Pairs = Pairs.OrderBy(t => t, StringComparer.Ordinal).ToList();

            Pairs = MargePairs(Pairs);
            List<string> mergedList = MargeTerms(TermsArray);
            string FilePath = IRSettings.Default.Destination + "\\Indexer" + FileCount;
            if (!File.Exists(FilePath))
                File.Create(FilePath).Close();
            using (StreamWriter sw = new StreamWriter(FilePath))
            {
                foreach (string line in mergedList)
                    sw.WriteLine(line);
            }
            FilePath = IRSettings.Default.Destination + "\\Pairs" + FileCount;
            if (!File.Exists(FilePath))
                File.Create(FilePath).Close();
            using (StreamWriter sw = new StreamWriter(FilePath))
            {
                foreach (string line in Pairs)
                    sw.WriteLine(line);
            }
            FileCount++;
        }

        /// <summary>
        /// marge pairs of terms and count how many times the pair apeared
        /// </summary>
        /// <param name="Pairs">list of pairs</param>
        /// <returns>marged list of pairs</returns>
        private List<string> MargePairs(List<string> Pairs)
        {
            List<string> margedPairs = new List<string>();
            string previousPair = "";
            int counter = 0;
            foreach (string p in Pairs)
            {
                if (previousPair == "")
                {
                    previousPair = p;
                    counter++;
                }
                else
                {
                    if (p == previousPair)
                        counter++;
                    else
                    {
                        margedPairs.Add(previousPair + "#" + counter);
                        previousPair = p;
                        counter = 1;
                    }
                }
            }
            margedPairs.Add(previousPair + "#" + counter);
            return margedPairs;
        }
        /// <summary>
        /// this method uses merge sort algoritem to marge all indexer files two at a time untill there is one file with all the data,
        /// and then splites the data into two files - one with the inverter index dictionary and one for the posting.
        /// for finish calls the func DeleteUnnecessaryFiles() to delete all the used files. 
        /// </summary>
        public void MergeAll()
        {
            int counter = 0;
            int NumOfFiles = (FileCount * 2) - 1;
            while (FileCount < NumOfFiles)
            {
                MergeTwo(counter, "Indexer");
                MergeTwo(counter, "Pairs");
                counter += 2;
                FileCount++;
            }
            SavePostingAndDictionary();
            SaveTop5Pairs();
        }

        private void SaveTop5Pairs()
        {
            string stemming;
            if (IRSettings.Default.Stemming)
                stemming = "-WithStemming";
            else
                stemming = "-WithoutStemming";
            if (!File.Exists(IRSettings.Default.Destination + "\\Pairs" + stemming))
                File.Create(IRSettings.Default.Destination + "\\Pairs" + stemming).Close();

            using (StreamWriter pairs = new StreamWriter(IRSettings.Default.Destination + "\\Pairs" + stemming))
            {
                using (StreamReader sr = new StreamReader(IRSettings.Default.Destination + "\\Pairs" + (FileCount - 1)))
                {
                    List<Tuple<string, int>> pairsOfTerm = new List<Tuple<string, int>>();
                    string previousTerm = "";
                    List<string> topPair;
                    string pairLine;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] splitedLine = line.Split('~');
                        if (previousTerm != "" && previousTerm != splitedLine[0])
                        {
                            topPair = findTop5(pairsOfTerm);
                            pairLine = previousTerm + "~";
                            foreach (string t in topPair)
                                pairLine += t + "|";
                            pairs.WriteLine(pairLine);
                            pairsOfTerm.Clear();
                        }
                        previousTerm = splitedLine[0];
                        string[] split = splitedLine[1].Split('#');
                        pairsOfTerm.Add(new Tuple<string, int>(split[0], Convert.ToInt32(split[1])));

                    }
                    topPair = findTop5(pairsOfTerm);
                    pairLine = previousTerm + "~";
                    foreach (string t in topPair)
                        pairLine += t + "|";
                    pairs.WriteLine(pairLine);
                    pairsOfTerm.Clear();
                }

            }
            File.Delete(IRSettings.Default.Destination + "\\Pairs" + (FileCount - 1));

        }

        private List<string> findTop5(List<Tuple<string, int>> pairsOfTerm)
        {
            List<string> top5 = new List<string>();
            pairsOfTerm = pairsOfTerm.OrderBy(p => p.Item2).ToList<Tuple<string, int>>();
            pairsOfTerm.Reverse();
            foreach (Tuple<string, int> item in pairsOfTerm)
            {
                if (top5.Count < 5)
                    top5.Add(item.Item1);
                else
                    break;
            }
            return top5;
        }



        /// <summary>
        /// this method seperate the merged files into posting file and dictionart file
        /// </summary>
        private void SavePostingAndDictionary()
        {
            string stemming;
            if (IRSettings.Default.Stemming)
                stemming = "-WithStemming";
            else
                stemming = "-WithoutStemming";
            if (!File.Exists(IRSettings.Default.Destination + "\\Posting" + stemming))
                File.Create(IRSettings.Default.Destination + "\\Posting" + stemming).Close();
            if (!File.Exists(IRSettings.Default.Destination + "\\Dictionary" + stemming))
                File.Create(IRSettings.Default.Destination + "\\Dictionary" + stemming).Close();

            using (StreamWriter posting = new StreamWriter(IRSettings.Default.Destination + "\\Posting" + stemming))
            {
                using (StreamWriter dictionary = new StreamWriter(IRSettings.Default.Destination + "\\Dictionary" + stemming))
                {
                    using (StreamReader sr = new StreamReader(IRSettings.Default.Destination + "\\Indexer" + (FileCount - 1)))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            string[] spliteLine = line.Split('#');
                            string term = spliteLine[0];
                            string[] docs = spliteLine[1].Split('|');
                            int df = docs.Length;
                            int tf = 0;
                            foreach (string doc in docs)
                                tf += doc.Split(' ').Length - 1;
                            numOfUniqueTerms++;
                            dictionary.WriteLine(term + "|" + tf + "|" + df);
                            //string postingline = "";
                            List<string> postingline = new List<string>();
                            foreach (string d in docs)
                            {
                                string[] appearns = d.Split(' ');
                                string docNum = appearns[0];
                                string[] split = appearns[1].Split(',');
                                //first location in doc
                                string location = split[0];
                                int maxWight= Convert.ToInt32(split[1]);
                                int appearens = 1;
                                //max wight in document
                                for (int i = 2; i < appearns.Length; i++)
                                {
                                    split = appearns[i].Split(',');
                                    maxWight = Math.Max(maxWight ,Convert.ToInt32(split[1]));
                                    appearens++;
                                }
                                postingline.Add( docNum + " " + location + " " + maxWight +" " +appearens +"|");
                            }
                            foreach (string p in postingline)
                            {
                                posting.Write(p);
                            }
                            posting.WriteLine("");
                            postingline.Clear();
                        }
                    }
                }
            }
            File.Delete(IRSettings.Default.Destination + "\\Indexer" + (FileCount - 1));
        }

        private void MergeTwo(int counter, string type)
        {
            string FilePath = IRSettings.Default.Destination + "\\" + type + FileCount;
            if (!File.Exists(FilePath))
                File.Create(FilePath).Close();
            using (StreamReader sr1 = new StreamReader(IRSettings.Default.Destination + "\\" + type + counter))
            {
                using (StreamReader sr2 = new StreamReader(IRSettings.Default.Destination + "\\" + type + (counter + 1)))
                {
                    using (StreamWriter sw = new StreamWriter(IRSettings.Default.Destination + "\\" + type + FileCount))
                    {
                        START:
                        string Line1 = sr1.ReadLine();
                        string Line2 = sr2.ReadLine();

                        while (Line1 != null && Line2 != null)
                        {
                            string[] Line1Splite = Line1.Split('#');
                            string[] Line2Splite = Line2.Split('#');
                            if (Line1Splite[0] == Line2Splite[0])
                            {
                                string mergeLine;
                                switch (type)
                                {
                                    case "Pairs":
                                        mergeLine = Line1Splite[0] + "#" + (Convert.ToInt32(Line1Splite[1]) + Convert.ToInt32(Line2Splite[1]));
                                        sw.WriteLine(mergeLine);
                                        break;
                                    case "Indexer":
                                        string posting = Line1Splite[1] + "|" + Line2Splite[1];
                                        mergeLine = Line1Splite[0] + "#" + posting;
                                        sw.WriteLine(mergeLine);
                                        break;
                                }
                                goto START;
                            }
                            else
                            {
                                if (string.Compare(Line1Splite[0], Line2Splite[0], StringComparison.Ordinal) < 0)
                                {
                                    sw.WriteLine(Line1);
                                    Line1 = sr1.ReadLine();
                                }
                                else
                                {
                                    sw.WriteLine(Line2);
                                    Line2 = sr2.ReadLine();
                                }
                            }
                        }
                        while (Line1 != null) //sr2 End Of Stream move all s1 lines to the file
                        {
                            sw.WriteLine(Line1);
                            Line1 = sr1.ReadLine();
                        }
                        while (Line2 != null) //sr1 End Of Stream move all s2 lines to the file
                        {
                            sw.WriteLine(Line2);
                            Line2 = sr2.ReadLine();
                        }
                    }
                }
            }
            File.Delete(IRSettings.Default.Destination + "\\" + type + counter);
            File.Delete(IRSettings.Default.Destination + "\\" + type + (counter + 1));
        }
        /// <summary>
        /// this method gets a list of sorted terms, and group all identical terms (with the same value). 
        /// multiple lines with identical terms -> one line with the terms value and all the document numbers and locations that this term appears in.
        /// </summary>
        /// <param name="SortedTerms">array of terms sorted lexicographic with repeats</param>
        /// <returns>array of terms sorted lexicographic with no repeats</returns>
        private List<string> MargeTerms(ParsedTerm[] SortedTerms)
        {
            List<string> margedTerms = new List<string>();
            int TermIndex = 0;
            ParsedTerm CurrentTerm = SortedTerms[0];

            START:
            string DocNum_Location_Weight = CurrentTerm.DocNum + " " + CurrentTerm.Location + "," + CurrentTerm.Weight;
            string CurrentTermValue = CurrentTerm.Value;
            string LastDocNum = CurrentTerm.DocNum;

            for (TermIndex++; TermIndex < SortedTerms.Length; TermIndex++)
            {
                ParsedTerm t = SortedTerms[TermIndex];
                if (t.Value == CurrentTermValue)
                {
                    if (LastDocNum == t.DocNum)
                        DocNum_Location_Weight += " " + t.Location + "," + t.Weight;
                    else
                    {
                        DocNum_Location_Weight += "|" + t.DocNum + " " + t.Location + "," + t.Weight;
                        LastDocNum = t.DocNum;
                    }
                }
                else
                {
                    margedTerms.Add(CurrentTermValue + "#" + DocNum_Location_Weight);
                    CurrentTerm = t;
                    goto START;
                }
            }
            margedTerms.Add(CurrentTermValue + "#" + DocNum_Location_Weight); //add last line
            return margedTerms;
        }
    }
}