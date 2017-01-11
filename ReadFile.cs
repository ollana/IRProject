
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace IRProject
{
    /// <summary>
    //this department store read documents.The department knows to receive a folder path which at all files in. 
    /// </summary>
    public class ReadFile
    {
        int counter = 0;
        string docspath;
        Parse parser;
        List<Tuple<string, int>> docText;
        List<string> languages = new List<string>();
        /// <summary>
        /// constractor - iterates all files in the given corpus path and send each one to ReadThisFile method.
        /// </summary>
        /// <param name="path"></param>
        public ReadFile(string path)
        {
            docText = new List<Tuple<string, int>>();
            docspath = path;
            string[] files = Directory.GetFiles(path);//get all files in the folder
            parser = new Parse(path + "\\stop_words.txt");
            foreach (string DocumentPath in files) //iterate all documents
            {
                if (!DocumentPath.Contains("stop_words"))
                    ReadThisFile(DocumentPath);
            }
            parser.SaveLastTerms();
        }

        /// <summary>
        /// returns the number of documents 
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfDocuments()
        {
            return parser.l_documents.Count;
        }

        /// <summary>
        /// returns number of uniqe terms from the parser 
        /// </summary>
        /// <returns></returns>
        public int GetNumOfUniqeTerms()
        {
            return parser.GetNumOfUniqeTerms();
        }

        /// <summary>
        /// this method gets a document path and read it line by line and catalogue to document categorys
        /// </summary>
        /// <param name="DocumentPath">the document full path</param>
        private void ReadThisFile(string DocumentPath)
        {
            counter++;
            string docNo, docDate, docTitle, docArtType, docLang; //documents details
            string tempText;
            char[] spase = { ' ' }; //for split function
            bool inText = false;

            if (File.Exists(DocumentPath))
            {
                StreamReader sr = new StreamReader(DocumentPath);
                emptyStrings:
                docNo = string.Empty;
                docDate = string.Empty;
                docTitle = string.Empty;
                docText.Clear();//list of text and it wight
                docArtType = string.Empty;
                docLang = string.Empty;
                inText = false;
                tempText = string.Empty;

                if (sr.EndOfStream) return;
                string line = sr.ReadLine().Trim();

                while (line != null)
                {
                    //split line to check the tag 
                    string[] SplitLine = line.Split(spase, StringSplitOptions.RemoveEmptyEntries);
                    if (SplitLine.Length < 1)
                    {
                        if (sr.EndOfStream) return;
                        line = sr.ReadLine().Trim();
                        continue;
                    }
                    switch (SplitLine[0])
                    {
                        case "<DOCNO>":
                            docNo = SplitLine[1].Trim();
                            line = sr.ReadLine().Trim();
                            break;
                        case "<DATE1>":
                            for (int s = 1; s < SplitLine.Length - 1; s++)
                            {
                                docDate += SplitLine[s] + " ";
                            }
                            line = sr.ReadLine().Trim();
                            break;
                        case "<H1>":
                        case "<H2>":
                        case "<H3>":
                        case "<H4>":
                        case "<H5>":
                        case "<H6>":
                        case "<H7>":
                        case "<H8>":
                            if (SplitLine[1] == "<TI>")
                                goto case "<TI>";
                            if (line != string.Empty)
                            {
                                if (tempText != string.Empty)
                                {
                                    docText.Add(new Tuple<string, int>(tempText.ToLower().Trim(), 0));
                                    tempText = string.Empty;
                                }
                                string wight = line.Trim().Substring(2, 1);
                                line = line.Replace("<H" + wight + ">", "");

                                while (!line.EndsWith("</H" + wight + ">"))
                                {
                                    tempText += line.Trim() + " ";
                                    line = sr.ReadLine().Trim();
                                }
                                line = line.Replace("</H" + wight + ">", "");
                                tempText += line.Trim() + " ";
                                docText.Add(new Tuple<string, int>(tempText.ToLower().Trim(), Convert.ToInt32(wight)));
                                tempText = string.Empty;
                                line = sr.ReadLine().Trim();
                            }
                            break;
                        case "<TI>":
                            line = line.Replace("<H3>", "").Replace("<TI>","");
                            while (!line.EndsWith("</H3>"))
                            {
                                docTitle += line.Trim() + " ";
                                line = sr.ReadLine().Trim();
                            }
                            line = line.Replace("</H3>", "").Replace("</TI>", "");
                            docTitle += line.Trim() + " ";
                            docText.Add(new Tuple<string, int>(docTitle.ToLower().Trim(), 9));
                            line = sr.ReadLine().Trim();
                            break;
                        case "Language:":
                            char[] a = { ' ', '>' };
                            docLang = (line.Split(a, StringSplitOptions.RemoveEmptyEntries)[3]).Replace(",","").ToLower();
                            string capitalWord = docLang.Substring(0, 1).ToUpper();
                            docLang = capitalWord+docLang.Substring(1);
                            if (docLang == "Enlgish"||docLang=="Eng")
                                docLang = "English";
                            if (docLang == "Arabi")
                                docLang = "Arabic";
                            if (docLang == "Span")
                                docLang = "Spanish";
                            if (docLang == "Slovene")
                                docLang = "Slovenian";
                            line = sr.ReadLine().Trim();
                            break;
                        case "Article":
                            if (SplitLine.Length > 1 && SplitLine[1].StartsWith("Type:"))
                                docArtType = line.Split(':')[1];
                            else goto default;
                            line = sr.ReadLine().Trim();
                            break;
                        case "<TEXT>":
                            inText = true;
                            line = sr.ReadLine().Trim();
                            break;
                        case "<F":
                            if (inText)
                            {
                                SplitLine = line.Split(spase, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 2; i < SplitLine.Length; i++)
                                {
                                    if (SplitLine[i] == "</F>") continue;
                                    tempText += SplitLine[i] + " ";
                                }
                            }
                            line = sr.ReadLine().Trim();
                            break;
                        case "[Text]":
                            line = line.Replace("[Text]", "");
                            while (line != "</TEXT>")
                            {
                                if (line.StartsWith("<H"))
                                {
                                    docText.Add(new Tuple<string, int>(tempText.ToLower().Trim(), 0));
                                    tempText = string.Empty;

                                    string wight = line.Trim().Substring(2, 1);
                                    line = line.Replace("<H" + wight + ">", "");

                                    while (!line.EndsWith("</H" + wight + ">"))
                                    {
                                        tempText += line.Trim() + " ";
                                        line = sr.ReadLine().Trim();
                                    }
                                    line = line.Replace("</H" + wight + ">", "");
                                    tempText += line.Trim() + " ";
                                    docText.Add(new Tuple<string, int>(tempText.ToLower().Trim(), Convert.ToInt32(wight)));
                                    tempText = string.Empty;
                                }
                                else if (line != string.Empty)
                                    tempText += line.Trim() + " ";
                                line = sr.ReadLine().Trim();
                            }
                            break;
                        case "</TEXT>":
                            if (sr.EndOfStream) return;
                            line = sr.ReadLine().Trim();
                            break;
                        case "</DOC>":
                            if (tempText != string.Empty)    
                                    docText.Add(new Tuple<string, int>(tempText.ToLower().Trim(), 0));
                            Document doc=new Document(docNo, docDate, docTitle, docArtType, docLang);
                            parser.ParseDoc(docText, doc);
                            goto emptyStrings;
                        default:
                            if (inText)
                            {
                                if (line != string.Empty)
                                    tempText += line.Trim() + " ";
                            }
                            line = sr.ReadLine().Trim();
                            break;
                    }
                }
                sr.Close();               
            }
        }
    }
}