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
            // DocNumber + "|" + Language + "|" + DocDate + "|" + max_tf + "|" + numOfUniqueWord + "|" + length;
            string[] info = docInfo.Split('|');
            m_DocNumber = info[0];
            m_language = info[1];
            m_DocDate = info[2];
            m_max_tf = Convert.ToInt32( info[3]);
            m_numOfUniqueWord = Convert.ToInt32(info[4]);
            m_length = Convert.ToInt32(info[5]);
            m_lineNumber = lineNumber;

        }

        public string DocumentNumber { get { return m_DocNumber; }  }

        public int Length { set { m_length = value; } get { return m_length; } }

        public int NumberUniqueWords { set { m_numOfUniqueWord = value; } get { return m_numOfUniqueWord; } }

        public int MaxTF { set { m_max_tf = value; } get { return m_max_tf; } }

        public string Language { get { return m_language; } }

        public double Rank { set { m_rank = value; } get { return m_rank; } }

        public int Index { get { return m_lineNumber; } }
        

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
