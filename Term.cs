using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRProject.Terms
{
    class Term
    {
        string term;
        string docNumber;
        int location;
        bool saved;
        bool isNumeric=false;
        int weight;

        /// <summary>
        /// create a term
        /// </summary>
        /// <param name="t">term </param>
        /// <param name="doc_num">document number</param>
        public Term(string t ,string doc_num)
        {
            term = t;
            checkIfNumber();
            docNumber = doc_num;
            saved = false;
        }



        //checks if the term is a number and update the numeric flag
        private void checkIfNumber()
        {
            double n;
            isNumeric = Double.TryParse(term, out n);

        }
        /// <summary>
        /// add the fraction to the term
        /// </summary>
        /// <param name="numerator"> numerator</param>
        /// <param name="denominator"> denominator</param>
        public void AddFraction(double numerator, double denominator)
        {
            if (isNumeric)
                term = Math.Round(Convert.ToDouble(term) +numerator / denominator,4)+ "";
            else
                term = Math.Round(numerator / denominator,4) + "";
            isNumeric = true;
        }
        /// <summary>
        /// the term
        /// </summary>
        public string Value
        {
            get { return term; }
            set { term = value; }

        }
        /// <summary>
        /// weigth of term
        /// </summary>
        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }
        /// <summary>
        /// document number of the term
        /// </summary>
        public string DocNum
        {
            get { return docNumber; }
        }
        /// <summary>
        /// location on document
        /// </summary>
        public int Location
        {
            get { return location; }
            set { location=value; }
        }
        /// <summary>
        /// is term already saved
        /// </summary>
        public bool Saved
        {
            get { return saved; }
            set { saved = value; }
        }
        /// <summary>
        /// is term numeric
        /// </summary>
        public bool IsNumeric
        {
            get
            {
                checkIfNumber();
                return isNumeric;
            }
        }
    }
}
