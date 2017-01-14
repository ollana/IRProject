using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRProject
{
    class ParsedTerm :Term
    {
        string m_docNumber;
        int m_location;
        bool m_saved;
        bool m_isNumeric = false;
        int m_weight;

        public int Weight  { get { return m_weight; } set { m_weight = value; } }
        public string DocNum  { get { return m_docNumber; }}
        public int Location { get { return m_location; } set { m_location = value; } }
        public bool Saved {get { return m_saved; }set { m_saved = value; }  }
        public bool IsNumeric {   get{checkIfNumber();return m_isNumeric;} }

        /// <summary>
        /// create a term
        /// </summary>
        /// <param name="t">term </param>
        /// <param name="doc_num">document number</param>
        public ParsedTerm(string t, string doc_num)
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
                m_term = Math.Round(Convert.ToDouble(m_term) + numerator / denominator, 4) + "";
            else
                m_term = Math.Round(numerator / denominator, 4) + "";
            m_isNumeric = true;
        }
    }
}
