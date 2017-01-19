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
        public int TF { get { return m_tf; } }
        public int DF { get { return m_df; } }
        public int LineNumber { get { return m_line; } }



        public DictionaryTerm(string term, int tf, int df, int line)
        {
            m_term = term;
            m_tf = tf;
            m_df = df;
            m_line = line;

        }
    }
}
