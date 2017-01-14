using IRProject.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRProject.Ranker
{
    class Ranker
    {
        List<Term> _termsOfQuery;
        Dictionary<string, double> _DocumentsScoure;
        double _AverageDocLength;
        int _NumberOfDocuments;
        
        string _Query;

        public Ranker(List<QueryTerm> Query, Document doc)
        {
            _DocumentsScoure = new Dictionary<string, double>();
            _termsOfQuery = new List<Term>();
            _NumberOfDocuments = IRSettings.Default.NumberOfDocuments;
            _AverageDocLength = IRSettings.Default.AverageDocLength;
            _Query = query;
            _termsOfQuery = Query;
        }
        
    }
}
