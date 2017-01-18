using System;
using System.Collections.Generic;

namespace IRProject.Ranker
{
    class Ranker
    {
        List<QueryTerm> _Query;
        BM25 bm;
        Document _doc;
        public Ranker()
        {
            bm = new BM25(1.2, 100, 0.75, 0, 0);
        }
        public double Rank(List<QueryTerm> Query, Document doc)
        {
            _doc = doc;
            _Query = Query;
            double bmRank = bm.Score(doc, _Query);
            double placeRank = PlaceRank();
            double wigthRank = TagRank();
            double tfidfRank = tfIdfRank();
            return 0.8* tfidfRank+ 0 *bmRank+0.1*placeRank+0.1*wigthRank;
        }

        private double tfIdfRank()
        {
            double rank = 0;
            foreach (QueryTerm q in _Query)
            {
                if (q.AppearsInDoc(_doc.DocumentNumber))
                {
                    double idf = Math.Log(IRSettings.Default.NumberOfDocuments/q.Term.DF);
                  //  double tf = (q.NumberOfAppearance(_doc.DocumentNumber) / _doc.MaxTF);
                    double tf =(q.NumberOfAppearance(_doc.DocumentNumber)/_doc.Length);
                    
                    rank += tf*idf;
                }
            }
            return rank/_Query.Count;
        }

        private double PlaceRank()
        {
            double rank = 0;
            foreach (QueryTerm q in _Query)
            {
                if (q.AppearsInDoc(_doc.DocumentNumber))
                {
                    rank += 1 - (1 / ((_doc.Length / q.FirstAppearens(_doc.DocumentNumber))));
                }
            }
            return rank/_Query.Count;
        }

        private double TagRank()
        {
            double rank = 0;
            foreach (QueryTerm q in _Query)
            {
                if (q.AppearsInDoc(_doc.DocumentNumber))
                    rank += q.MaxWight(_doc.DocumentNumber);
            }
            return (rank / (9 * _Query.Count));
        }
    }
}
