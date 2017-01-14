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
        public QueryTerm(Term term, int count, string postingData)
        {
            m_term = term;
            m_count = count;
        }


    }
}