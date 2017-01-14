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
         Dictionary<string, Term> m_dictionary;
         Dictionary<string, List<string>> m_pairs;
        Dictionary<string, Document> m_documents;
         Dictionary<string,List<string>> m_docInLanglanguages;
        Parse m_parser;

        /// <summary>
        /// constractor
        /// </summary>
        public Searcher(string docPath)
        {
            m_dictionary = new Dictionary<string, Term>();
            m_pairs = new Dictionary<string, List<string>>();
            m_parser = new Parse(docPath);
            m_documents = new Dictionary<string, Document>();
            m_docInLanglanguages = new Dictionary<string, List<string>>();
        }
        /// <summary>
        /// returns list of 5 top options to suggest
        /// </summary>
        /// <param name="word">word</param>
        /// <returns>list of suggestion words</returns>
        public List<string> AutoComplete(string word)
        {
            List<string> complete = new List<string>();
            if (m_pairs.ContainsKey(word))
                complete = m_pairs[word];
            return complete;
        }

        /// <summary>
        /// returns the top 50 documents matchimg a given query ranked by relevance
        /// </summary>
        /// <param name="query"><query/param>
        /// <returns>list of top 50 documents</returns>
         public List<string> Search(string query, List<string> langlanguages)
        {
            List<string> relevant_docs = new List<string>();
            List<string> parsedTerms = m_parser.ParseQuery(query);
            List<Tuple<Term, int>> termsInQuery = new List<Tuple<Term, int>>();
            List<string> termsAdded = new List<string>();
            //for each term in the query check if it appears in the dictionary, if so count 
            foreach (string t in parsedTerms)
            {
                if (m_dictionary.ContainsKey(t)&& !termsAdded.Contains(t))
                {
                    termsInQuery.Add(new Tuple<Term, int>(m_dictionary[t], parsedTerms.Count(item => item == t)));
                    termsAdded.Add(t);
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

        /// <summary>
        /// load all data
        /// </summary>
        /// <param name="dictionaryPath">dictionary file path</param>
        /// <param name="pairsFilePath">pairs file path</param>
        /// <param name="documentsDataPath">documents file path</param>
        /// <param name="langueges">list of langueges</param>
        public void LoadDictionaries(string dictionaryPath, string pairsFilePath, string documentsDataPath, List<string> langueges)
        {
            LoadDictionary(dictionaryPath);
            LoadPairs(pairsFilePath);
            LoadDocuments(documentsDataPath);
            LoadDocInLanguege(langueges);

        }
        public void ResetDictionaries()
        {
            m_dictionary.Clear();
            m_docInLanglanguages.Clear() ;
            m_documents.Clear();
            m_pairs.Clear();
        }

        /// <summary>
        /// save for every languege list of documents in that languege
        /// </summary>
        /// <param name="languegeslist"> list of langueges</param>
        private void LoadDocInLanguege(List<string> languegeslist)
        {
            List<string> langueges = new List<string>();

            foreach (string l in languegeslist)
            {
                string languege = l.Replace("\r", "");
                langueges.Add(languege);
                m_docInLanglanguages.Add(languege, new List<string>());
            }

            foreach (var d in m_documents)
            {
                if(langueges.Contains(d.Value.DocumentLanguage))
                    m_docInLanglanguages[d.Value.DocumentLanguage].Add(d.Key);
            }
        }

        /// <summary>
        /// load dictionary from file
        /// </summary>
        /// <param name="path">file path</param>
        private void LoadDictionary(string path)
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
                        m_dictionary.Add(split[0], new Term(split[0], Convert.ToInt32(split[1]), Convert.ToInt32(split[2]), lineNum));
                    }
                    line = sr.ReadLine();
                    lineNum++;
                }
            }


        }
        /// <summary>
        /// load pairs from file
        /// </summary>
        /// <param name="path">file path</param>
        private void LoadPairs(string path)
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
                        m_pairs.Add(split[0], words);
                    }
                    line = sr.ReadLine();
                    lineNum++;
                }
            }

        }

        /// <summary>
        /// load documents from file
        /// </summary>
        /// <param name="path"> file path</param>
        private void LoadDocuments(string path)
        {

            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                string line = sr.ReadLine();

                while (line != null)
                {
                    if (line != string.Empty)
                    {
                        string[] split = line.Split('|');
                        m_documents.Add(split[0], new Document(line));
                    }
                    line = sr.ReadLine();
                }
            }


        }
    }
}
