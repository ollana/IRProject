using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IRProject.Ranker;

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
            m_ranker = new Ranker.Ranker();
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
                if (m_pairs.ContainsKey(word.ToLower()))
                    complete = m_pairs[word.ToLower()];
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
        public List<string> Search(string query, List<string> languages,int queryNum)
        {
            if (m_loaded)
            {
                //list of terms in query
                List<QueryTerm> termsInQuery = FindQueryTerms(query);
                //list of documents to rank
                List<string> docOfQuery = new List<string>();
                foreach (QueryTerm q in termsInQuery)
                {
                    foreach (string doc in q.GetDocumentsOfTerm())
                    {
                        if (!docOfQuery.Contains(doc))
                            docOfQuery.Add(doc);
                    }
                }
                List<Document> docToRank = FindDocumentsToRank(languages,docOfQuery);
                //rate each document
                foreach (Document d in docToRank)
                {

                    d.Rank=m_ranker.Rank(termsInQuery, d);
                }
                List<string> top50= FindTop50Docs(docToRank);
                SaveResults(queryNum, top50);
                return top50;
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

        private void SaveResults(int queryNum, List<string> docs)
        {
            string filePath = @"C:\tr\results.txt";
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            using (StreamWriter pairs = new StreamWriter(filePath,true))
            {
                foreach (string d in docs)
                {

                    pairs.WriteLine(queryNum + " " + 0 + " " + d + " " + 0 + " "+0+" mt");
                }
            }
        }

        /// <summary>
        /// documents in the given langueges
        /// </summary>
        /// <param name="languages">langueges</param>
        /// <returns> list of documents according to the given langueges </returns>
        private List<Document> FindDocumentsToRank(List<string> languages,List<string> docs)
        {
            List<Document> docToRank = new List<Document>();
            if (!languages.Contains("All"))
            {
                foreach (string lan in languages)
                {
                    foreach (string d in m_docInLanglanguages[lan])
                    {
                        if (docs.Contains(d) && !docToRank.Contains(m_documents[d]))
                            docToRank.Add(m_documents[d]);
                    }
                }
            }
            else
            {
                //add all documents
                foreach (var d in docs)
                {
                    docToRank.Add(m_documents[d]);
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
                    using (FileStream sr = new FileStream(m_postingPath, FileMode.Open, FileAccess.Read))
                    //using (StreamReader sr = new StreamReader(m_postingPath))
                    {
                        sr.Position = m_dictionary[t].StartPosition;
                        //sr.Seek(m_dictionary[t].StartPosition, SeekOrigin.Begin);
                        //line = File.ReadLines(m_postingPath).Skip(m_dictionary[t].LineNumber).Take(1).First();
                        byte[] b = new byte[m_dictionary[t].EndPosition-m_dictionary[t].StartPosition];
                        sr.Read(b, 0, (int)(m_dictionary[t].EndPosition - m_dictionary[t].StartPosition));

                        line = System.Text.Encoding.UTF8.GetString(b);
                        

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
            m_postingPath = postingPath.Replace("\n","").Replace("\r","");
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
                        m_dictionary.Add(split[0], new DictionaryTerm(split[0], Convert.ToInt32(split[1]), Convert.ToInt32(split[2]), lineNum, Convert.ToInt64(split[3]), Convert.ToInt64(split[4])));
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
            int totalLengt = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                int docIndex = 0;
                while (line != null)
                {
                    if (line != string.Empty)
                    {
                        string[] split = line.Split('|');
                        Document doc = new Document(line, docIndex);
                        totalLengt += doc.Length;
                        m_documents.Add(split[0], doc);
                        docIndex++;
                    }
                    line = sr.ReadLine();
                }
                IRSettings.Default.AverageDocLength = totalLengt / docIndex;
                IRSettings.Default.NumberOfDocuments = docIndex;
            }


        }
    }
}
