using System;
using System.Collections.Generic;

namespace IRProject
{
     class QueryTerm
    {
        public Term Term { get { return m_term; }  }
        public int Count { get { return m_count; } }
        public Dictionary<string, List<Tuple<int, int>>> Documents { get { return m_termDocuments; } }

        Term m_term;
        int m_count;
        Dictionary<string,List<Tuple <int, int>>> m_termDocuments;

        /// <summary>
        /// constractor;
        /// </summary>
        /// <param name="term">term </param>
        /// <param name="count">times appeared in query</param>
        /// <param name="postingData">posting information </param>
        public QueryTerm(Term term, int count, string postingData)
        {
            m_term = term;
            m_count = count;
            m_termDocuments = new Dictionary<string, List<Tuple<int, int>>>();
            string[] docs = postingData.Split('|');
            foreach (string doc in docs)
            {
                List<Tuple<int, int>> locationsWigths = new List<Tuple<int, int>>();
                string[] appearns = doc.Split(' ');
                string docNum = appearns[0];
                for(int i=1;i<appearns.Length;i++)
                {
                    string[] split = appearns[i].Split(',');
                    int location =Convert.ToInt32(split[0]);
                    int weight=Convert.ToInt32(split[1]);
                    locationsWigths.Add(new Tuple<int, int>(location, weight));
                }
                m_termDocuments.Add(docNum, locationsWigths);
            }

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
                return m_termDocuments[docNum].Count;
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
                return m_termDocuments[docNum].Count;
            }
            throw new Exception("Term not appears in the given Document");

        }
        public int FirstAppearens(string docNum)
        {
            if (AppearsInDoc(docNum))
            {
            }

            throw new Exception("Term not appears in the given Document");

        }




    }
}