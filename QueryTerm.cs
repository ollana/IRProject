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
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="count"></param>
        /// <param name="postingData"></param>
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


    }
}