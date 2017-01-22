using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IRProject.Ranker;
using Newtonsoft.Json;
using System.Net;

namespace IRProject
{
    public class Searcher
    {
        Dictionary<string, DictionaryTerm> m_dictionary;
        Dictionary<string, List<string>> m_pairs;
        Dictionary<string, Document> m_documents;
        Dictionary<string, List<string>> m_docInLanglanguages;
        Parse m_parser;
        Ranker.Ranker m_ranker;
        string m_postingPath;
        bool m_loaded;
        public bool Loaded { get { return m_loaded; } }

        /// <summary>
        /// constractor
        /// </summary>
        public Searcher()
        {
            m_dictionary = new Dictionary<string, DictionaryTerm>();
            m_pairs = new Dictionary<string, List<string>>();
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
            word = word.ToLower().Trim();
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
        public List<string> Search(string query, List<string> languages)
        {
            query = query.ToLower().Trim();
            if (m_loaded)
            {
                List<int> linesToGet = new List<int>();
                List<QueryTerm> queryTerms= FindQueryTerms(query);
                foreach (QueryTerm q in queryTerms)
                {
                    q.Wigth = 0.5 / queryTerms.Count;
                    linesToGet.Add(q.Term.LineNumber);
                }
                string semanticQuery = "";
                string[] queryWord = query.Split(' ');
                foreach (string word in queryWord)
                {
                    List<string> semanticofWord = GetSemantic(word, 5);
                    foreach (string w in semanticofWord)
                    {
                        semanticQuery += w + " ";
                    }
                }
                List<QueryTerm> semanticQueryTerms = FindQueryTerms(semanticQuery);
                foreach (QueryTerm q in semanticQueryTerms)
                {
                    q.Wigth = 0.5 / semanticQueryTerms.Count;
                    linesToGet.Add(q.Term.LineNumber);
                    queryTerms.Add(q);
                }
                if (queryTerms.Count == 0)
                    return new List<string>();
                Dictionary<int,string> lines=GetPostingInformationForTerms(linesToGet);
                foreach (QueryTerm q in queryTerms)
                {
                    q.SetPostingData(lines[q.Term.LineNumber]);
                }
                return FindRelevantDocsToQuery(queryTerms, languages);
            }


            else throw new Exception("Dictionary not loaded");
        }

        /// <summary>
        /// find relevant documents to query
        /// </summary>
        /// <param name="termsInQuery"></param>
        /// <param name="languages"></param>
        /// <returns>top 50 documents</returns>
        private List<string> FindRelevantDocsToQuery(List<QueryTerm> termsInQuery, List<string> languages)
        {
            //list of documents to rank
            HashSet<string> docOfQuery = new HashSet<string>();
            foreach (QueryTerm q in termsInQuery)
            {
                foreach (string doc in q.GetDocumentsOfTerm())
                {
                    if (!docOfQuery.Contains(doc))
                        docOfQuery.Add(doc);
                }
            }
            HashSet<Document> docToRank = FindDocumentsToRankByLanguege(languages, docOfQuery);
            //rate each document
            foreach (Document d in docToRank)
            {

                d.Rank = m_ranker.Rank(termsInQuery, d);
            }
            List<string> top50 = FindTop50Docs(addSimilarDocuments(docToRank));
            return top50;
        }

        /// <summary>
        /// for the documents that get the higher rank, add similar documents and give them a rank acoording to their simularity
        /// </summary>
        /// <param name="docToRank"> documents</param>
        /// <returns> ranked documents </returns>
        private Dictionary<string,double> addSimilarDocuments(HashSet<Document> docToRank)
        {
            Dictionary<string, double> documentesInSet = new Dictionary<string, double>();
            List<Document> rankedDocuments = docToRank.OrderBy(p => p.Rank).ToList<Document>();
            rankedDocuments.Reverse();
            int count = 0;
            foreach (var item in rankedDocuments)
            {
                documentesInSet.Add(item.DocumentNumber,item.Rank);
            }
            foreach (Document Doc in rankedDocuments)
            {
                if (count > 15) break;
                count++;
                foreach (var similarDoc in Doc.SimilarDocuments)
                {
                    double similarityRank = 0.8*Doc.Rank * similarDoc.Item2;

                    if (!documentesInSet.ContainsKey(similarDoc.Item1))
                        documentesInSet.Add(similarDoc.Item1,similarityRank);
                    else
                        documentesInSet[similarDoc.Item1] += similarityRank;
                }
            }
            return documentesInSet;
        }

        /// <summary>
        /// find and return top 50 ranked documents
        /// </summary>
        /// <param name="docToRank">documents</param>
        /// <returns>list of top 50 docs</returns>
        private List<string> FindTop50Docs(Dictionary<string, double> docToRank)
        {
            List<Tuple<string, double>> documents = new List<Tuple<string, double>>();
            foreach (var item in docToRank)
            {
                documents.Add(new Tuple<string,double>( item.Key, item.Value));
            }
            documents = documents.OrderBy(d => d.Item2).ToList();
            documents.Reverse();
            List<string> topdocs = new List<string>();
            foreach (var d in documents)
            {
                if (topdocs.Count < 50)
                    topdocs.Add(d.Item1);
                
            }
            return topdocs;
        }

      
        /// <summary>
        /// documents in the given langueges
        /// </summary>
        /// <param name="languages">langueges</param>
        /// <returns> list of documents according to the given langueges </returns>
        private HashSet<Document> FindDocumentsToRankByLanguege(List<string> languages,HashSet<string> docs)
        {
            HashSet<Document> docToRank = new HashSet<Document>();
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
                    termsInQuery.Add(new QueryTerm(m_dictionary[t], parsedTerms.Count(item => item == t)));
                    termsAdded.Add(t);
                }

            }
            return termsInQuery;
        }

        /// <summary>
        /// load the posting information needed for query
        /// </summary>
        /// <param name="lineNumbers"> liens numbers to load </param>
        /// <returns> nessecary  posting data </returns>
        private Dictionary<int,string> GetPostingInformationForTerms(List<int> lineNumbers)
        {
            Dictionary<int, string> postingOfLines = new Dictionary<int, string>();
            string line;
            lineNumbers.Sort();
            //take the term information from the posting file by order
            using (StreamReader sr = new StreamReader(m_postingPath))
            {
                int last= lineNumbers.Last();
                for (int i = 1; i <= last; i++)
                {
                    if (lineNumbers.Contains(i))
                    {
                        line = sr.ReadLine();
                        postingOfLines.Add(i, line);
                    }
                    else
                        sr.ReadLine();
                }

            }
            return postingOfLines;
        }


        /// <summary>
        /// load all data
        /// </summary>
        /// <param name="dictionaryPath">dictionary file path</param>
        /// <param name="pairsFilePath">pairs file path</param>
        /// <param name="documentsDataPath">documents file path</param>
        /// <param name="postingPath">posting file path</param>
        /// <param name="langueges">list of langueges</param>
        public void LoadDictionaries(string dictionaryPath, string pairsFilePath, string documentsDataPath,string postingPath,string stopWordsPath,string docSimilarityPath, List<string> langueges)
        {
            m_parser = new Parse(stopWordsPath);
            LoadDictionary(dictionaryPath);
            LoadPairs(pairsFilePath);
            LoadDocuments(documentsDataPath);
            LoadDocumentsSimilarity(docSimilarityPath);
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
                int lineNum = 1;

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

        /// <summary>
        /// returns the dictionary with tf for each term
        /// </summary>
        /// <returns>dictionary of terms with tf for each term</returns>
        public Dictionary<string, int> getDictionary()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            foreach (var term in m_dictionary)
            {
                dic.Add(term.Key,term.Value.TF);
            }
            return dic;
        }

        /// <summary>
        /// load similar documents for each doc
        /// </summary>
        /// <param name="path">file path</param>
        private void LoadDocumentsSimilarity(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line != string.Empty)
                    {
                        string[] split = line.Split('#');
                        if(split.Length>1 && split[1]!="" && m_documents.ContainsKey(split[0]))
                            m_documents[split[0]].SetSimilarDocuments(split[1]);
                    }
                    line = sr.ReadLine();
                }
               
            }

        }

        /// <summary>
        /// get semantic words ot ferm
        /// </summary>
        /// <param name="term"> term</param>
        /// <param name="max">number of words </param>
        /// <returns>list of semantic words</returns>
        private List<string> GetSemantic(string term, int max)
        {
            List<string> l = new List<string>();
            string name;
            using (WebClient wc = new WebClient())
            {
                name = wc.DownloadString("http://api.datamuse.com/words?ml=" + term + "&max=" + max);
            }
            dynamic d = JsonConvert.DeserializeObject(name);
            foreach (dynamic item in d)
            {
                l.Add(item.word.ToString());
            }

            return l;
        }
    }
}
