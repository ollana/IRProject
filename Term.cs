using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRProject
{
    public class Term
    {
        string m_term;
        string m_docNumber;
        int m_location;
        bool m_saved;
        bool m_isNumeric=false;
        int m_weight;

        private int m_tf;
        private int m_df;
        private int m_line;

        public Term(string term, int tf, int df, int line)
        {
            m_term = term;
            m_tf = tf;
            m_df = df;
            m_line = line;
        }

        /// <summary>
        /// create a term
        /// </summary>
        /// <param name="t">term </param>
        /// <param name="doc_num">document number</param>
        public Term(string t ,string doc_num)
        {
            m_term = t;
            checkIfNumber();
            m_docNumber = doc_num;
            m_saved = false;
        }



        //checks if the term is a number and update the numeric flag
        private void checkIfNumber()
        {
            double n;
            m_isNumeric = Double.TryParse(m_term, out n);

        }
        /// <summary>
        /// add the fraction to the term
        /// </summary>
        /// <param name="numerator"> numerator</param>
        /// <param name="denominator"> denominator</param>
        public void AddFraction(double numerator, double denominator)
        {
            if (m_isNumeric)
                m_term = Math.Round(Convert.ToDouble(m_term) +numerator / denominator,4)+ "";
            else
                m_term = Math.Round(numerator / denominator,4) + "";
            m_isNumeric = true;
        }
        /// <summary>
        /// the term
        /// </summary>
        public string Value
        {
            get { return m_term; }
            set { m_term = value; }

        }
        /// <summary>
        /// weigth of term
        /// </summary>
        public int Weight
        {
            get { return m_weight; }
            set { m_weight = value; }
        }
        /// <summary>
        /// document number of the term
        /// </summary>
        public string DocNum
        {
            get { return m_docNumber; }
        }
        /// <summary>
        /// location on document
        /// </summary>
        public int Location
        {
            get { return m_location; }
            set { m_location=value; }
        }
        /// <summary>
        /// is term already saved
        /// </summary>
        public bool Saved
        {
            get { return m_saved; }
            set { m_saved = value; }
        }
        /// <summary>
        /// is term numeric
        /// </summary>
        public bool IsNumeric
        {
            get
            {
                checkIfNumber();
                return m_isNumeric;
            }
        }
        public int TF
        {
            get { return m_tf; }
        }
        public int DF
        {
            get { return m_df; }
        }
        public int LineNumber
        {
            get { return m_line; }
        }
    }
}
