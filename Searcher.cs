using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRProject
{
    public class Searcher
    {
         public Dictionary<string, Term> Dictionary;
         public Dictionary<string, List<string>> Pairs;

         public List<string> AutoComplete(string word)
        {
            List<string> complete = new List<string>();

            return complete;
        }

         public List<string> Search(string query)
        {
            List<string> relevant_docs = new List<string>();

            return relevant_docs;

        }

         public void LoadDictionary(string path)
        {
            Dictionary = new Dictionary<string, Term>();

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
            Pairs = new Dictionary<string, List<string>>();
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
