using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRProject
{
    public class Searcher
    {
         public Dictionary<string, Term> Dictionary;
         public Dictionary<string, List<string>> Pairs;
        Parse Parser;

        /// <summary>
        /// constractor
        /// </summary>
        public Searcher(string docPath)
        {
            Dictionary = new Dictionary<string, Term>();
            Pairs = new Dictionary<string, List<string>>();
            Parser = new Parse(docPath);
        }
        /// <summary>
        /// returns list of 5 top options to suggest
        /// </summary>
        /// <param name="word">word</param>
        /// <returns>list of suggestion words</returns>
        public List<string> AutoComplete(string word)
        {
            List<string> complete = new List<string>();
            if (Pairs.ContainsKey(word))
                complete = Pairs[word];
            return complete;
        }

        /// <summary>
        /// returns the top 50 documents matchimg a given query ranked by relevance
        /// </summary>
        /// <param name="query"><query/param>
        /// <returns>list of top 50 documents</returns>
         public List<string> Search(string query)
        {
            List<string> relevant_docs = new List<string>();
            List<string> parsedTerms = Parser.ParseQuery(query);
            List<Tuple<Term, int>> termsInQuery = new List<Tuple<Term, int>>();
            //for each term in the query check if it appears in the dictionary, if so count 
            foreach (string t in parsedTerms)
            {
                if (Dictionary.ContainsKey(t))
                {
                    termsInQuery.Add(new Tuple<Term, int>(Dictionary[t], parsedTerms.Count(item => item == t)));
                    parsedTerms.Remove(t);
                }
                
            }



            //if (File.Exists(path))
            //{
            //    //using (StreamReader sr = new StreamReader(path))
            //    //{
            //    //    string line = File.ReadLines(path).Skip(14).Take(1).First();

            //    //}
            //}
             

            return relevant_docs;
        }
          //  public Rank(List<Tuple<string, int>> WordsInQueary,List<Term> terms, Document doc,List<Tuple<int,int>> LocationsAndWaigthInDoc...)


         public void LoadDictionary(string path)
        {

            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                string line = sr.ReadLine();
                int lineNum = 0;

                while (line != null)
                {
                    if (line != string.Empty)
                    {
                        string[] split = line.Split('|');
                        Dictionary.Add(split[0], new Term(split[0], Convert.ToInt32(split[1]), Convert.ToInt32(split[2]), lineNum));
                    }
                    line = sr.ReadLine();
                    lineNum++;
                }
            }


        }
        public void LoadPairs(string path)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                string line = sr.ReadLine();
                int lineNum = 0;

                while (line != null)
                {
                    if (line != string.Empty)
                    {
                        string[] split = line.Split('~');
                        List<string> words = new List<string>();
                        char[] c = { '|' };
                        string[] pairs = split[1].Split(c, StringSplitOptions.RemoveEmptyEntries);
                        if (pairs.Length > 0)
                        {
                            foreach (string pair in pairs)
                            {
                                words.Add(pair);
                            }
                        }
                        Pairs.Add(split[0], words);
                    }
                    line = sr.ReadLine();
                    lineNum++;
                }
            }

        }
    }
}
