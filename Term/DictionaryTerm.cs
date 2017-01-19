using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRProject
{
    class DictionaryTerm:Term
    {
        private int m_tf;
        private int m_df;
        private int m_line;
        private long m_start, m_end;
        public int TF { get { return m_tf; } }
        public int DF { get { return m_df; } }
        public int LineNumber { get { return m_line; } }
        public long StartPosition { get { return m_start; } }
        public long EndPosition { get { return m_end; } }



        public DictionaryTerm(string term, int tf, int df, int line,long start,long end)
        {
            m_term = term;
            m_tf = tf;
            m_df = df;
            m_line = line;
            m_start = start;
            m_end=end;

        }
    }
}
