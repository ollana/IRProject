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
            
        }
        public double Rank(List<QueryTerm> Query, Document doc)
        {
            bm = new BM25(1.2, 100, 0.75, 0, 0);
            _doc = doc;
            _Query = Query;
            double bmRank = bm.Score(doc, _Query);
            double placeRank = PlaceRank();
            double wigthRank = TagRank();
            double cossim = CosSim();
            return 0.5* cossim+ 0 *bmRank+0.4*placeRank+0.1*wigthRank;
        }

        private double CosSim()
        {
            double rank = 0;
            int lengthOfQuery = 0;
            double WijWiq_sum = 0, Wij_sq_sum = 0, Wiq_sq_sum = 0;
            foreach (QueryTerm q in _Query)
                lengthOfQuery += q.Count;
            foreach (QueryTerm q in _Query)
            {
                double Wiq = q.Count / lengthOfQuery;
                Wiq_sq_sum += Math.Pow(Wiq, 2);
                if (q.AppearsInDoc(_doc.DocumentNumber))
                {
                    double idf = Math.Log(IRSettings.Default.NumberOfDocuments / q.Term.DF);
                    //double tf = (q.NumberOfAppearance(_doc.DocumentNumber) / _doc.MaxTF);
                    double tf = ((double)q.NumberOfAppearance(_doc.DocumentNumber) / _doc.Length);
                    double Wij = tf * idf;
                    Wij_sq_sum += Math.Pow(Wij, 2);
                    WijWiq_sum += Wij*Wiq;
                    rank += tf * idf;
                }
            }
           // rank = WijWiq_sum / (Math.Sqrt(Wij_sq_sum * Wiq_sq_sum));
            return rank/_Query.Count;
        }

        private double PlaceRank()
        {
            double rank = 0;
            foreach (QueryTerm q in _Query)
            {
                if (q.AppearsInDoc(_doc.DocumentNumber))
                {
                    rank += 1 - (1 / (((double)_doc.Length / q.FirstAppearens(_doc.DocumentNumber))));
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
