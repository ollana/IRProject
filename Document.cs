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

        string DocNumber, Titel, ArticleType, Language,DocDate;
        int max_tf, numOfUniqueWord,length;

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
            DocNumber = docNo;
            string[] date = docDate.Split(' ');
            DocDate = docDate;

            Titel = docTitle;
            ArticleType = docArtType;
            Language = docLang;
        }
        public Document(string docInfo)
        {
            // DocNumber + "|" + Language + "|" + DocDate + "|" + max_tf + "|" + numOfUniqueWord + "|" + length;
            string[] info = docInfo.Split('|');
            DocNumber = info[0];
            Language = info[1];
            DocDate = info[2];
            max_tf = Convert.ToInt32( info[3]);
            numOfUniqueWord = Convert.ToInt32(info[4]);
            length = Convert.ToInt32(info[5]);

        }
        /// <summary>
        /// the documenr number
        /// </summary>
        public string DocumentNumber { get { return DocNumber; }  }
        /// <summary>
        /// the documenr length
        /// </summary>
        public int DocumentLength { set { length = value; } get { return length; } }
        /// <summary>
        /// the documenr number of unique words
        /// </summary>
        public int DocumentNumberUniqueWords { set { numOfUniqueWord = value; } get { return numOfUniqueWord; } }
        /// <summary>
        /// the documenr max term frequency
        /// </summary>
        public int DocumentMaxTF { set { max_tf = value; } get { return max_tf; } }

        public string DocumentLanguage { get { return Language; } }


        /// <summary>
        /// discription of the document the document
        /// </summary>
        /// <returns> string describes the document</returns>
        public override string ToString()
        {
            return DocNumber + "|" + Language + "|" + DocDate + "|" + max_tf + "|" + numOfUniqueWord + "|" + length;
        } 
    }

}
