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

        string m_DocNumber, Titel, ArticleType, m_language,m_DocDate;
        int max_tf, numOfUniqueWord,length;
        double m_rank;
        /// <summary>
        /// constractor, constract a document and save his attributes
        /// </summary>
        /// <param name="docNo">document number</param>
        /// <param name="docDate">document date</param>
        /// <param name="docTitle">document titels</param>
        /// <param name="docArtType">document art type</param>
        /// <param name="docLang">document length</param>
        public Document(string docNo, string docDate, string docTitle, string docArtType, string docLang)
        {
            length = 0;
            m_DocNumber = docNo;
            string[] date = docDate.Split(' ');
            m_DocDate = docDate;

            Titel = docTitle;
            ArticleType = docArtType;
            m_language = docLang;
        }
        public Document(string docInfo)
        {
            // DocNumber + "|" + Language + "|" + DocDate + "|" + max_tf + "|" + numOfUniqueWord + "|" + length;
            string[] info = docInfo.Split('|');
            m_DocNumber = info[0];
            m_language = info[1];
            m_DocDate = info[2];
            max_tf = Convert.ToInt32( info[3]);
            numOfUniqueWord = Convert.ToInt32(info[4]);
            length = Convert.ToInt32(info[5]);

        }
        /// <summary>
        /// the documenr number
        /// </summary>
        public string DocumentNumber { get { return m_DocNumber; }  }
        /// <summary>
        /// the documenr length
        /// </summary>
        public int Length { set { length = value; } get { return length; } }
        /// <summary>
        /// the documenr number of unique words
        /// </summary>
        public int NumberUniqueWords { set { numOfUniqueWord = value; } get { return numOfUniqueWord; } }
        /// <summary>
        /// the documenr max term frequency
        /// </summary>
        public int MaxTF { set { max_tf = value; } get { return max_tf; } }

        public string Language { get { return m_language; } }

        public double Rank { set { m_rank = value; } get { return m_rank; } }



        /// <summary>
        /// discription of the document the document
        /// </summary>
        /// <returns> string describes the document</returns>
        public override string ToString()
        {
            return m_DocNumber + "|" + m_language + "|" + m_DocDate + "|" + max_tf + "|" + numOfUniqueWord + "|" + length;
        } 
    }

}
