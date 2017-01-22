using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRProject
{
    class Document
    {

        string m_DocNumber, m_language,m_DocDate;
        int m_max_tf, m_numOfUniqueWord,m_length,m_lineNumber;
        double m_rank;
        DateTime m_date;
        List<Tuple<string, double>> m_similarDoc;
        public static DateTime MaxDate = new DateTime(1995, 6, 24);
        public static DateTime MinDate = new DateTime(1984, 3, 3);

        /// <summary>
        /// constractor, constract a document and save his attributes
        /// </summary>
        /// <param name="docNo">document number</param>
        /// <param name="docDate">document date</param>
        /// <param name="docLang">document length</param>
        public Document(string docNo, string docDate, string docLang)
        {
            m_length = 0;
            m_DocNumber = docNo;
            string[] date = docDate.Split(' ');
            m_DocDate = docDate;
            
            m_language = docLang;
        }
        public Document(string docInfo,int lineNumber)
        {
            string[] info = docInfo.Split('|');
            m_DocNumber = info[0];
            m_language = info[1];
            DateTime.TryParse(info[2],out m_date);
            m_max_tf = Convert.ToInt32( info[3]);
            m_numOfUniqueWord = Convert.ToInt32(info[4]);
            m_length = Convert.ToInt32(info[5]);
            m_similarDoc = new List<Tuple<string, double>>();
            m_lineNumber = lineNumber;

        }
        /// <summary>
        /// save the most similar documents for this document with pracentage of similarity
        /// </summary>
        /// <param name="line"></param>
        public void SetSimilarDocuments(string line)
        {
            char[] c = { '|' };
            string[] docs = line.Split(c, StringSplitOptions.RemoveEmptyEntries);
            int numOfDocsAdded = 0;
            foreach (string d in docs)
            {
                string[] split = d.Split(' ');
                double precentage;
                if (double.TryParse(split[1], out precentage))
                {
                    m_similarDoc.Add(new Tuple<string, double>(split[0], precentage));
                    numOfDocsAdded++;
                }
                if (numOfDocsAdded == 5) break;
            }
        }
        public List<Tuple<string,double>> SimilarDocuments { get { return m_similarDoc; } }

        public string DocumentNumber { get { return m_DocNumber; }  }

        public int Length { set { m_length = value; } get { return m_length; } }

        public int NumberUniqueWords { set { m_numOfUniqueWord = value; } get { return m_numOfUniqueWord; } }

        public int MaxTF { set { m_max_tf = value; } get { return m_max_tf; } }

        public string Language { get { return m_language; } }

        public double Rank { set { m_rank = value; } get { return m_rank; } }

        public int Index { get { return m_lineNumber; } }
        public DateTime Date { get { return m_date; } }


        /// <summary>
        /// discription of the document the document
        /// </summary>
        /// <returns> string describes the document</returns>
        public override string ToString()
        {
            return m_DocNumber + "|" + m_language + "|" + m_DocDate + "|" + m_max_tf + "|" + m_numOfUniqueWord + "|" + m_length;
        } 
    }

}
