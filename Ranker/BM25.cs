using System;
using System.Collections.Generic;

namespace IRProject.Ranker
{
    class BM25
    {
        double _k1, _k2, _b, _K;
        int _ri, _R;
        double _avgdl;
        int _N;

        public BM25( double k1, double k2, double b, int R, int ri)
        {
            _k1 = k1;
            _k2 = k2;
            _b = b;
            _ri = ri;
            _R = R;
            _N = IRSettings.Default.NumberOfDocuments;
            _avgdl = IRSettings.Default.AverageDocLength;

        }

        /// <summary>
        /// this method clcluate the BM25 scoure for a list of documents and a query.
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public double Score(Document d, List<QueryTerm> Query)
        {
            double sum = 0;
            foreach (QueryTerm q in Query)
            {
                if(q.AppearsInDoc(d.DocumentNumber))
                    sum += ScoreOne(d.Length, q.DocumentFrequency(), q.NumberOfAppearance(d.DocumentNumber), q.Count)*q.Wigth;
            }
            return sum;
        }

        /// <summary>
        /// BN25 inner sigma loop
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public double ScoreOne(double _dl, double _ni, double _fi, double _qfi)
        {
            _K = _k1 * ((_b * (_dl /_avgdl)) + (1 - _b));
            double toLog = ((_ri + 0.5) / (_R - _ri + 0.5)) / ((_ni - _ri + 0.5) / (_N - _ni - _R + _ri + 0.5));
            double docK = ((_k1 + 1) * _fi) / (_K + _fi);
            double queK = ((_k2 + 1) * _qfi) / (_k2 + _qfi);
            return (Math.Log(toLog) * docK * queK);
        }
    }
}
