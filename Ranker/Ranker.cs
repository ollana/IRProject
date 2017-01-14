using System.Collections.Generic;

namespace IRProject.Ranker
{
    class Ranker
    {
        List<QueryTerm> _Query;
        BM25 bm;
        Document _doc;
        public Ranker(List<QueryTerm> Query)
        {
            _Query = Query;
            bm = new BM25(1.2, 100, 0.75, 0, 0);
        }
        public double Rank(Document doc)
        {
            _doc = doc;
            double DocRank = bm.Score(doc, _Query);
            DocRank += PlaceRank();
            DocRank += TagRank();
            return DocRank;
        }

        public double PlaceRank()
        {
            double rank = 0;
            foreach (QueryTerm q in _Query)
            {
                if (q.AppearsInDoc(_doc.DocumentNumber))
                {
                    rank += 1 - (1 / ((_doc.Length / q.FirstAppearens(_doc.DocumentNumber))));
                }
            }
            return rank;
        }

        public double TagRank()
        {
            double rank = 0;
            foreach (QueryTerm q in _Query)
            {
                rank += q.MaxWight(_doc.DocumentNumber);
            }
            return (rank / (9 * _Query.Count));
        }
    }
}
