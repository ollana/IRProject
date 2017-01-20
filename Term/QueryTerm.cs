using System;
using System.Collections.Generic;

namespace IRProject
{
     class QueryTerm
    {
        public DictionaryTerm Term { get { return m_term; }  }
        public int Count { get { return m_count; } }
        public double Wigth { get { return m_wigth; } set { m_wigth = value; } }

        DictionaryTerm m_term;
        int m_count;
        double m_wigth;
        //document-first location, max wight, number of appearens
        Dictionary<string,Tuple <int, int,int>> m_termDocuments;

        /// <summary>
        /// constractor;
        /// </summary>
        /// <param name="term">term </param>
        /// <param name="count">times appeared in query</param>
        /// <param name="postingData">posting information </param>
        public QueryTerm(DictionaryTerm term, int count)
        {
            m_term = term;
            m_count = count;
            m_termDocuments = new Dictionary<string, Tuple<int, int, int>>();
            

        }

        public void SetPostingData(string postingData)
        {
            char[] c = { '|' };
            string[] docs = postingData.Split(c, StringSplitOptions.RemoveEmptyEntries);
            foreach (string doc in docs)
            {
                char[] s = { ' ' };
                string[] appearns = doc.Split(s, StringSplitOptions.RemoveEmptyEntries);
                string docNum = appearns[0];
                int location = Convert.ToInt32(appearns[1]);
                int wight = Convert.ToInt32(appearns[2]);
                int appearens = Convert.ToInt32(appearns[3]);
                m_termDocuments.Add(docNum, new Tuple<int, int, int>(location, wight, appearens));
            }
        }

        /// <summary>
        /// returns Df of the term
        /// </summary>
        /// <returns></returns>
        public int DocumentFrequency()
        {
            return m_termDocuments.Count;
        }
        /// <summary>
        /// check if the term apeears in this document
        /// </summary>
        /// <param name="docNum">document number</param>
        /// <returns>true if appears in doc</returns>
        public bool AppearsInDoc(string docNum)
        {
            return m_termDocuments.ContainsKey(docNum);
        }

        /// <summary>
        /// returns how many times term appears in document
        /// </summary>
        /// <param name="docNum">document number</param>
        /// <returns>number of appearance</returns>
        public int NumberOfAppearance(string docNum)
        {
            if (AppearsInDoc(docNum))
                return m_termDocuments[docNum].Item3;
            throw new Exception("Term not appears in the given Document");
        }

        /// <summary>
        /// max wight of the term in document
        /// </summary>
        /// <param name="docNum"></param>
        /// <returns></returns>
        public int MaxWight(string docNum)
        {
            if (AppearsInDoc(docNum))
            {
                return m_termDocuments[docNum].Item2;
            }
            throw new Exception("Term not appears in the given Document");

        }
        /// <summary>
        /// first appearens of the term in document
        /// </summary>
        /// <param name="docNum">document number</param>
        /// <returns> first appearens</returns>
        public int FirstAppearens(string docNum)
        {
            if (AppearsInDoc(docNum))
            {
                return m_termDocuments[docNum].Item1;
            }

            throw new Exception("Term not appears in the given Document");

        }
        public List<string> GetDocumentsOfTerm()
        {
            List<string> docs = new List<string>();
            foreach (var d in m_termDocuments)
            {
                docs.Add(d.Key);
            }
            return docs;
        }




    }
}