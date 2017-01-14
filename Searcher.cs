using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IRProject
{
    public class Searcher
    { 
        Dictionary<string, DictionaryTerm> m_dictionary;
        Dictionary<string, List<string>> m_pairs;
        Dictionary<string, Document> m_documents;
        Dictionary<string,List<string>> m_docInLanglanguages;
        Parse m_parser;
        Ranker.Ranker m_ranker;
        string m_postingPath;
        bool m_loaded;
        public bool Loaded { get { return m_loaded; } }


        /// <summary>
        /// constractor
        /// </summary>
        public Searcher(string docPath)
        {
            m_dictionary = new Dictionary<string, DictionaryTerm>();
            m_pairs = new Dictionary<string, List<string>>();
            m_parser = new Parse(docPath);
            m_documents = new Dictionary<string, Document>();
            m_docInLanglanguages = new Dictionary<string, List<string>>();
            m_loaded = false;
        }
        /// <summary>
        /// returns list of 5 top options to suggest
        /// </summary>
        /// <param name="word">word</param>
        /// <returns>list of suggestion words</returns>
        public List<string> AutoComplete(string word)
        {
            if (m_loaded)
            {
                List<string> complete = new List<string>();
                if (m_pairs.ContainsKey(word))
                    complete = m_pairs[word];
                return complete;
            }
            else throw new Exception("dictionary not loaded");
        }

        /// <summary>
        /// returns the top 50 documents matchimg a given query ranked by relevance
        /// </summary>
        /// <param name="query"><query/param>
        /// <param name="languages"><languages/param> 
        /// <returns>list of top 50 documents</returns>
        public List<string> Search(string query, List<string> languages)
        {
            if (m_loaded)
            {
                //list of terms in query
                List<QueryTerm> termsInQuery = FindQueryTerms(query);
                //list of documents to rank
                List<Document> docToRank = FindDocumentsToRank(languages);
                //rate each document
                foreach (Document d in docToRank)
                {

                    d.Rank=m_ranker.Rank(termsInQuery, d);
                }

                return FindTop50Docs(docToRank);
            }
            else throw new Exception("Dictionary not loaded");
        }
        /// <summary>
        /// find and return top 50 ranked documents
        /// </summary>
        /// <param name="docToRank">documents</param>
        /// <returns>list of top 50 docs</returns>
        private List<string> FindTop50Docs(List<Document> docToRank)
        {
            List<string> topdocs = new List<string>();
            List<Document> rankedDoc = docToRank.OrderBy(p => p.Rank).ToList<Document>();
            rankedDoc.Reverse();
            foreach (Document d in rankedDoc)
            {
                if (topdocs.Count < 50)
                    topdocs.Add(d.DocumentNumber);
                else
                    break;
            }
            return topdocs;
        }

        /// <summary>
        /// documents in the given langueges
        /// </summary>
        /// <param name="languages">langueges</param>
        /// <returns> list of documents according to the given langueges </returns>
        private List<Document> FindDocumentsToRank(List<string> languages)
        {
            List<Document> docToRank = new List<Document>();
            if (!languages.Contains("All"))
            {
                foreach (string lan in languages)
                {
                    foreach (string d in m_docInLanglanguages[lan])
                    {
                        if (!docToRank.Contains(m_documents[d]))
                            docToRank.Add(m_documents[d]);
                    }
                }
            }
            else
            {
                //add all documents
                foreach (var d in m_documents)
                {
                    docToRank.Add(d.Value);
                }
            }

            return docToRank;
        }

        /// <summary>
        /// returns a list of the parsed terms in the queary and how many times it appeared
        /// </summary>
        /// <param name="query"> query </param>
        /// <returns>list of the terms and count</returns>
        private List<QueryTerm> FindQueryTerms(string query)
        {
            List<string> parsedTerms = m_parser.ParseQuery(query);
            List<QueryTerm> termsInQuery = new List<QueryTerm>();
            List<string> termsAdded = new List<string>();
            //for each term in the query check if it appears in the dictionary, if so count haw many times it apeear in the query
            foreach (string t in parsedTerms)
            {
                if (m_dictionary.ContainsKey(t) && !termsAdded.Contains(t))
                {
                    string line;
                    //take the term information from the posting file
                    using (StreamReader sr = new StreamReader(m_postingPath))
                    {
                        line = File.ReadLines(m_postingPath).Skip(m_dictionary[t].LineNumber).Take(1).First();

                    }
                    termsInQuery.Add(new QueryTerm(m_dictionary[t], parsedTerms.Count(item => item == t), line));
                    termsAdded.Add(t);
                }

            }
            return termsInQuery;
        }


        /// <summary>
        /// load all data
        /// </summary>
        /// <param name="dictionaryPath">dictionary file path</param>
        /// <param name="pairsFilePath">pairs file path</param>
        /// <param name="documentsDataPath">documents file path</param>
        /// <param name="postingPath">posting file path</param>
        /// <param name="langueges">list of langueges</param>
        public void LoadDictionaries(string dictionaryPath, string pairsFilePath, string documentsDataPath,string postingPath, List<string> langueges)
        {
            LoadDictionary(dictionaryPath);
            LoadPairs(pairsFilePath);
            LoadDocuments(documentsDataPath);
            LoadDocInLanguege(langueges);
            m_postingPath = postingPath;
            m_loaded = true;

        }
        /// <summary>
        /// reset all data
        /// </summary>
        public void ResetDictionaries()
        {
            m_dictionary.Clear();
            m_docInLanglanguages.Clear() ;
            m_documents.Clear();
            m_pairs.Clear();
            m_loaded = false;
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
                if(langueges.Contains(d.Value.Language))
                    m_docInLanglanguages[d.Value.Language].Add(d.Key);
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
                        m_dictionary.Add(split[0], new DictionaryTerm(split[0], Convert.ToInt32(split[1]), Convert.ToInt32(split[2]), lineNum));
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
                int lineNum = 0;
                while (line != null)
                {
                    if (line != string.Empty)
                    {
                        string[] split = line.Split('|');
                        m_documents.Add(split[0], new Document(line,lineNum));
                        lineNum++;
                    }
                    line = sr.ReadLine();
                }
            }


        }
    }
}
