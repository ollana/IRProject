using System.Collections.Generic;
using System.Linq;

namespace IRProject.Ranker
{
    class Jaccard
    {
        Dictionary<string,List<int>> DocTermsList;
        public Jaccard(string PostingPath)
        {
            DocTermsList = new Dictionary<string, List<int>>();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(PostingPath))
            {
                int termCounter = 0;
                string line = sr.ReadLine();
                while (line!=null)
                {
                    string[] docs = line.Split('|');
                    foreach (string s in docs)
                    {
                        string doc = s.Split(' ')[0];
                        if (!DocTermsList.ContainsKey(doc))
                            DocTermsList.Add(doc, new List<int>());
                        DocTermsList[doc].Add(termCounter); 
                    }
                    line = sr.ReadLine();
                    termCounter++;
                }
            }
        }

        public void JaccardToFile(string fileName,double rank)
        {
            if (!System.IO.File.Exists(fileName))
                System.IO.File.Create(fileName).Close();
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName))
            {
                foreach (string doc1 in DocTermsList.Keys)
                { 
                    foreach (string doc2 in DocTermsList.Keys)
                    {
                        double ans = Calc(DocTermsList[doc1], DocTermsList[doc2]);
                        if (ans >= rank)
                            sw.Write(doc2+" "+ans+"|");
                    }
                    sw.Write(sw.NewLine);
                }
            }
                
        }
        public double Calc(HashSet<int> hs1, HashSet<int> hs2)
        {
            return hs1.Intersect(hs2).Count() / (double)hs1.Union(hs2).Count();
        }

        public double Calc(List<int> ls1, List<int> ls2)
        {
            HashSet<int> hs1 = new HashSet<int>(ls1);
            HashSet<int> hs2 = new HashSet<int>(ls2);
            return Calc(hs1, hs2);
        }
    }
}
