using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRProject
{
    abstract public class Term
    {
        protected string m_term;
        public string Value{ get { return m_term; } set { m_term = value; }  }
        
       
    }
}
