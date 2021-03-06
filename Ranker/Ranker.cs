﻿using System;
using System.Collections.Generic;

namespace IRProject.Ranker
{
    class Ranker
    {
        List<QueryTerm> _Query;
        BM25 bm;
        Document _doc;
        /// <summary>
        /// rank the document relevancy to query
        /// </summary>
        /// <param name="Query">query</param>
        /// <param name="doc">doc</param>
        /// <returns>rank</returns>
        public double Rank(List<QueryTerm> Query, Document doc)
        {
            bm = new BM25(0.8, 0, 0.25, 0, 0);
            _doc = doc;
            _Query = Query;
            double bmRank = bm.Score(doc, _Query);
            double locationRank = LocationRank();
            double tagRank = TagRank();
            double tfidf = TfIdf();
            double cosSim = CosSim();
            double dateRank = DateRank();
            return  bmRank + 0.5 * (0.8 * tfidf + 0.2 * locationRank)  + 0.5 * dateRank + 0 * tagRank + 0 * cosSim;
        }

        /// <summary>
        /// tf idf
        /// </summary>
        /// <returns></returns>
        private double TfIdf()
        {
            double rank = 0;
            foreach (QueryTerm q in _Query)
            {
                if (q.AppearsInDoc(_doc.DocumentNumber))
                {
                    double idf = Math.Log(IRSettings.Default.NumberOfDocuments / q.Term.DF);
                    double tf = ((double)q.NumberOfAppearance(_doc.DocumentNumber) / _doc.Length);
                    double Wij = tf * idf;
                    rank += tf * idf*q.Wigth;
                }
            }
            return rank;
        }

        /// <summary>
        /// CosSim rank
        /// </summary>
        /// <returns>rank</returns>
        private double CosSim()
        {
            double rank = 0;
             double WijWiq_sum = 0, Wij_sq_sum = 0, Wiq_sq_sum = 0;
            
            foreach (QueryTerm q in _Query)
            {
                double Wiq = q.Wigth;
                Wiq_sq_sum += (double)Math.Pow(Wiq, 2);
                if (q.AppearsInDoc(_doc.DocumentNumber))
                {
                    double idf = Math.Log(IRSettings.Default.NumberOfDocuments / q.Term.DF);
                    double tf = ((double)q.NumberOfAppearance(_doc.DocumentNumber) / _doc.Length);
                    double Wij = tf * idf;
                    Wij_sq_sum += (double)Math.Pow(Wij, 2);
                    WijWiq_sum += Wij*Wiq;
                }
            }
            rank = WijWiq_sum / (Math.Sqrt(Wij_sq_sum * Wiq_sq_sum));
            return rank;
        }

        /// <summary>
        /// rank by first appearence in the document
        /// </summary>
        /// <returns>rank </returns>
        private double LocationRank()
        {
            double rank = 0;
            foreach (QueryTerm q in _Query)
            {
                if (q.AppearsInDoc(_doc.DocumentNumber))
                {
                    rank += 1 - (q.FirstAppearens(_doc.DocumentNumber) / (((double)_doc.Length)));
                }
            }
            return rank/_Query.Count;
        }

        /// <summary>
        ///  rank by first wigth of term in the document
        /// </summary>
        /// <returns></returns>
        private double TagRank()
        {
            double rank = 0;
            foreach (QueryTerm q in _Query)
            {
                if (q.AppearsInDoc(_doc.DocumentNumber))
                    rank += q.MaxWight(_doc.DocumentNumber);
            }
            return (rank /9 *_Query.Count );
        }

        private double DateRank()
        {
            double rank = 0;
            if (_doc.Date != null)
            {
                TimeSpan deltaFromDocTOmin = TimeSpan.FromTicks((_doc.Date.Subtract(Document.MinDate).Ticks));
                TimeSpan deltaFromMaxTOmin = TimeSpan.FromTicks((Document.MaxDate.Subtract(Document.MinDate).Ticks));
                rank = (double)deltaFromDocTOmin.Days / deltaFromMaxTOmin.Days;

            }
            return rank;
        }
    }
}
