using SearchEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace IRProject
{
    /// <summary>
    /// separate documents into terms
    /// </summary>
    class Parse
    {
        enum TYPE { Corpus,Query};
        const int NUMBER_OF_Terms_TO_INDEX = 100000;
        string[] TermsToSort;
        int weightOfText;
        int index;
        int location;
        string docNum;
        int UniqueTermsInDoc;
        int maxTFInDoc;
        Dictionary<string, int> termsInDoc;
        List<Term> terms;
        Stemmer stemmer;
        HashSet<string> stopWords;
        Indexer indexer;
        public List<Document> l_documents = new List<Document>();
        string previousTerm;
        List<string> pairs;
        TYPE ParseType;
        /// <summary>
        /// constructor, creates list of terms, stemmer, indexer and hash set of stop words  
        /// </summary>
        /// <param name="DocumentPath">path of the stop words file</param>
        public Parse(string DocumentPath)
        {
            terms = new List<Term>();
            stemmer = new Stemmer();
            stopWords = new HashSet<string>();
            indexer = new Indexer();
            pairs = new List<string>();
            LoadstopWords(DocumentPath);
            ParseType = TYPE.Query;

        }

        /// <summary>
        /// get number of unique terms
        /// </summary>
        /// <returns> number of unique terms</returns>
        public int GetNumOfUniqeTerms()
        {
            return indexer.UniqueTerms;
        }

        /// <summary>
        ///parse document, removes punctuation marks and split the text into terms(that will be sorted), every "NUMBER_OF_Terms_TO_INDEX"  sand the term list to the indexer 
        /// </summary>
        /// <param name="text">text </param>
        /// <param name="doc">document</param>
        public void ParseDoc(List<Tuple<string, int>> text, Document doc)
        {
            ParseType = TYPE.Corpus;
            index = 0;
            UniqueTermsInDoc = 0;
            maxTFInDoc = 0;
            termsInDoc = new Dictionary<string, int>();
            location = 1;
            docNum = doc.DocumentNumber;
            previousTerm = null;
            foreach (Tuple<string, int> part in text)
            {
                if (part.Item1 != null)
                {
                    string partToParse = RemovePunctuationMarks(part.Item1);
                    char[] splitby = { ' ' };
                    TermsToSort = partToParse.Split(splitby, StringSplitOptions.RemoveEmptyEntries);
                    weightOfText = part.Item2;
                    ParseTerms();
                    index = 0;
                    previousTerm = null;
                }
            }
            doc.DocumentLength = location--;
            doc.DocumentNumberUniqueWords = UniqueTermsInDoc;
            doc.DocumentMaxTF = maxTFInDoc;
            l_documents.Add(doc);
            //after parsing NUMBER_OF_DUCUMENTS_TO_INDEX documents, send the term list to the indexer and start a new list
            if (terms.Count >= NUMBER_OF_Terms_TO_INDEX)
            {
                indexer.Index(terms, pairs);
                terms.Clear();
                pairs.Clear();
            }

        }
        /// <summary>
        /// parse a query
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>list of terms in query</returns>
        public List<string> ParseQuery(string query)
        {
            ParseType = TYPE.Query;
            terms.Clear();
            index = 0;
            docNum = "";
            string partToParse = RemovePunctuationMarks(query);
            char[] splitby = { ' ' };
            TermsToSort = partToParse.Split(splitby, StringSplitOptions.RemoveEmptyEntries);
            ParseTerms();
            List<string> termsInQuery = new List<string>();
            foreach (Term t in terms)
            {
                termsInQuery.Add(t.Value);
            }
            terms.Clear();
            return termsInQuery;
        }

        /// <summary>
        /// load set of stop words to ignore 
        /// </summary>
        /// <param name="DocumentPath">Document Path </param>
        private void LoadstopWords(string DocumentPath)
        {
            if (File.Exists(DocumentPath))
            {
                StreamReader sr = new StreamReader(DocumentPath);
                if (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        //remove any 's (becouse it was removed from the text ass well)
                        stopWords.Add(line.Replace("'s", "").Replace("'", ""));
                        line = sr.ReadLine();
                    }
                }
                sr.Close();
            }
        }

        /// <summary>
        /// send each term to be parsed seperatrly, in the end update the number of document parsed
        /// </summary>
        private void ParseTerms()
        {
            //sort the terms
            while (index < TermsToSort.Length)
            {
                checkRangePhrase(TermsToSort[index]);
                index++;
            }
        }

        /// <summary>
        /// check if the term is a range phrase term- if so save it as united term(and each word seperatly), 
        /// if the term is not a phrase- parse it 
        /// </summary>
        private void checkRangePhrase(string term)
        {
            char[] splitby = { '-' };
            string[] termsphrase = term.Split(splitby, StringSplitOptions.RemoveEmptyEntries);
            //  (term-term) or (term-term-term)
            if (termsphrase.Length > 1 && termsphrase.Length < 4)
                RangePhraseTerm(termsphrase);
            //not a pattern of a range phrase, check each one seperatly
            else
            {
                for (int i = 0; i < termsphrase.Length; i++)
                    ParseTerm(termsphrase[i]);
            }
        }
        /// <summary>
        /// parse each term by rules- numbers, fractions, precentage, prices, dates, and ranges and add it to the term list
        /// </summary>
        /// <param name="termToParse"> term </param>
        private void ParseTerm(string termToParse)
        {
            if (termToParse.Length > 0)
            {
                Term term = new Term(termToParse, docNum);
                //if the term is a fraction or number followed by a fraction 
                checkFraction(term);
                checkNumber(term);
                if (!term.Saved)
                    checkPrecentage(term);
                if (!term.Saved)
                    checkPrice(term);
                if (!term.Saved)
                    checkDate(term);
                if (!term.Saved)
                    checkBetweenRange(term);
                if (!term.Saved)
                {
                    addSimpleTerm(term);
                }
            }
        }


        /// <summary>
        /// if the term is a number- check if the next word is a fraction- if so combine them to one number
        /// if the term is a fraction- save it as a number 
        /// </summary>
        /// <param name="term"> term </param>
        private void checkFraction(Term term)
        {
            //if the term is a number- check if the next word is a fraction- if so combine them
            if (term.IsNumeric && index < TermsToSort.Length - 1 && isWordFraction(TermsToSort[index + 1]))
            {
                string[] frac = TermsToSort[index + 1].Split('/');
                term.AddFraction(Convert.ToDouble(frac[0]), Convert.ToDouble(frac[1]));
                index++;
            }
            //if the word is a fraction save it as a number
            else if (isWordFraction(term.Value))
            {
                string[] frac = term.Value.Split('/');
                term.AddFraction(Convert.ToDouble(frac[0]), Convert.ToDouble(frac[1]));
            }
        }
        /// <summary>
        /// if the term is a number, convert it to millions and measure units(if nessesery)
        /// check the next word for special cases( like precent,price)
        /// check if its a part of a date
        /// </summary>
        /// <param name="term"> term</param>
        private void checkNumber(Term term)
        {
            if (term.IsNumeric)
            {
                //convert to "Million" expression if nessesery
                ConvertToMillion(term);
                //check the next word for special cases
                checkNextTerm(term);
                //convert to measure units if nessesery (if next term is a measure unit)
                ConvertToMeasureUnits(term);
                //check if the number is part of a date
                if (term.Value.Length <= 2)
                    checkDateTerm(term);
                addTermToTermList(term);
            }
        }



        /// <summary>
        ///  check if the term is precentage (number ends with %)
        /// </summary>
        /// <param name="term"> term</param>
        private void checkPrecentage(Term term)
        {
            //check if percentage
            if (term.Value[term.Value.Length - 1] == '%')
            {
                term.Value = term.Value.Substring(0, term.Value.Length - 1);
                if (term.IsNumeric)
                {
                    term.Value += "%";
                    addTermToTermList(term);
                }
                else if (term.Value.Length == 0)
                    term.Saved = true;
            }

        }

        /// <summary>
        /// check if the term is a price(number starting with $)- if so add "dollars" to it,
        /// </summary>
        /// <param name="term"> term</param>
        private void checkPrice(Term term)
        {
            if (term.Value[0] == '$')
            {
                term.Value = term.Value.Substring(1);
                if (term.IsNumeric)
                {
                    //convert to "Million" expression if nessesery
                    ConvertToMillion(term);
                    term.Value += " dollars";
                    addTermToTermList(term);
                }
                else if (term.Value.Length == 0)
                    term.Saved = true;
                if (term.Value.Contains("$"))
                    term.Value = term.Value.Replace("$", "");
            }

        }

        /// <summary>
        /// check if the term is a part of a date (if the term is name of a month or number ends with "th")
        /// </summary>
        /// <param name="term">term</param>
        private void checkDate(Term term)
        {
            string month = monthOfTerm(term.Value);
            //if the term is a month check if its followed a date if so marge it to one date term
            if (month != "0")
                makeDateStartWithMonth(month, term);

            else if (term.Value.Length > 2 && term.Value.Substring(term.Value.Length - 2) == "th")
            {
                string originalTerm = term.Value;
                term.Value = term.Value.Replace("th", "");
                //if the term is a one/two digit number followed by "th" check if its a part of a date phrase 
                if ((term.Value.Length == 1 || term.Value.Length == 2) && isTermNumber(term.Value))
                    checkDateTerm(term);
                if (term.Value != originalTerm.Replace("th", ""))
                    addTermToTermList(term);
                else
                    term.Value = originalTerm;
            }
        }

        /// <summary>
        /// check if the term is a range in the pattern (between number and number)
        /// </summary>
        /// <param name="term">term</param>
        private void checkBetweenRange(Term term)
        {
            int initialindex = index;
            if (term.Value == "between" && index < TermsToSort.Length - 3)
            {
                Term word2 = new Term(TermsToSort[index + 1], docNum);
                index++;
                checkFraction(word2);
                if (word2.IsNumeric)
                {
                    //convert to "Million" expression if nessesery
                    ConvertToMillion(word2);
                    if (index < TermsToSort.Length - 1 && TermsToSort[index + 1] == "and")
                    {
                        index++;
                        if (index < TermsToSort.Length - 1)
                        {
                            Term word4 = new Term(TermsToSort[index + 1], docNum);
                            index++;
                            checkFraction(word4);
                            if (word4.IsNumeric)
                            {
                                //convert to "Million" expression if nessesery
                                ConvertToMillion(word4);
                                term.Value = "between " + word2.Value + " and " + word4.Value;
                                addTermToTermList(term);
                                return;
                            }
                        }
                    }
                }
            }
            index = initialindex;
        }



        /// <summary>
        /// check if the term is not a stop word.
        /// if its a stop word dont save it
        /// if not stemm the term (if need) and add it to the term list
        /// </summary>
        /// <param name="term">term</param>
        /// <returns>true if term add to the term list</returns>
        private void addSimpleTerm(Term term)
        {
            if (!stopWords.Contains(term.Value))
            {
                if (IRSettings.Default.Stemming)
                    term.Value = stemmer.stemTerm(term.Value);
                addTermToTermList(term);
            }
        }
        /// <summary>
        /// add a term to the term list. 
        /// count for each term in the document how many times it appers
        /// if the term have the max frequancy for the document by far- save the max frequncy
        /// if term apears for the first time add one to the number of unique terms in the document, if its apers again reduce one.
        /// </summary>
        /// <param name="term">term </param>
        private void addTermToTermList(Term term)
        {
            if (ParseType==TYPE.Corpus && term.Value.Length > 0)
            {
                if (!termsInDoc.ContainsKey(term.Value))
                {
                    termsInDoc.Add(term.Value, 1);
                    UniqueTermsInDoc++;
                }
                else
                {
                    int fr = termsInDoc[term.Value];
                    termsInDoc[term.Value] = fr + 1;
                    maxTFInDoc = Math.Max(maxTFInDoc, fr + 1);
                }
                term.Location = location;
                term.Weight = weightOfText;
                terms.Add(term);
                term.Saved = true;
                location++;
                if (previousTerm != null)
                    pairs.Add(previousTerm + "~" + term.Value);
                previousTerm = term.Value;

            }
            else if(ParseType == TYPE.Query)
            {
                terms.Add(term);
                term.Saved = true;
            }
        }

        /// <summary>
        /// if month followed by a date(day,year or year only), marge the term to a date and save it
        /// </summary>
        private void makeDateStartWithMonth(string month, Term term)
        {
            if (index < TermsToSort.Length - 1)
            {
                string nextword = TermsToSort[index + 1];
                string year = "", day;
                if (isTermNumber(nextword))
                {
                    if (nextword.Length <= 2)
                    {
                        if (nextword.Length == 1) nextword = "0" + nextword;
                        day = nextword;
                        index++;
                        if (index < TermsToSort.Length - 1)
                        {
                            nextword = TermsToSort[index + 1];
                            if (isTermNumber(nextword) && nextword.Length == 4)
                            {
                                year = nextword + "-";
                                index++;
                            }
                        }
                        term.Value = (year + month + "-" + day);
                        addTermToTermList(term);
                    }
                    else if (nextword.Length == 4)
                    {
                        index++;
                        year = nextword + "-";
                        term.Value = (year + month);
                        addTermToTermList(term);
                    }
                }
            }
        }
        /// <summary>
        /// if the term is a month- convert it to the month number, else return "0"
        /// </summary>
        /// <param name="month">month</param>
        /// <returns>number representation</returns>
        private string monthOfTerm(string month)
        {
            switch (month)
            {
                case "january":
                case "jan":
                    return "01";
                case "february":
                case "feb":
                    return "02";
                case "march":
                case "mar":
                    return "03";
                case "april":
                case "apr":
                    return "04";
                case "may":
                    return "05";
                case "june":
                case "jun":
                    return "06";
                case "july":
                case "jul":
                    return "07";
                case "august":
                case "aug":
                    return "08";
                case "september":
                case "sep":
                    return "09";
                case "october":
                case "oct":
                    return "10";
                case "november":
                case "nov":
                    return "11";
                case "december":
                case "dec":
                    return "12";
                default:
                    return "0";

            }
        }

        /// <summary>
        /// sort by range phrase patterns- (word,word)(word-number)(number-word)(number-number)(word-word-word)
        /// if one of the patterns- save each term by itself and all together
        /// if not one of the paterns- continue checking each term indevidualy
        /// </summary>
        /// <param name="phrase"> term array</param>
        private void RangePhraseTerm(string[] phrase)
        {
            Term phraseTerm;
            string rangePhrase = "";
            switch (phrase.Length)
            {
                case (2):
                    for (int i = 0; i < 2; i++)
                    {
                        Term term = new Term(phrase[i], docNum);
                        checkFraction(term);
                        if (term.IsNumeric)
                            //convert to "Million" expression if nessesery
                            ConvertToMillion(term);

                        rangePhrase += term.Value;
                        //if not last
                        if (i < phrase.Length - 1)
                            rangePhrase += "-";
                    }
                    phraseTerm = new Term(rangePhrase, docNum);
                    addTermToTermList(phraseTerm);
                    break;
                case (3):
                    bool allWords = true;
                    for (int i = 0; i < 3; i++)
                    {
                        //check if the term is a number
                        allWords = allWords && !isTermNumber(phrase[i]);
                    }
                    //if all the term is words save it seperatly and togethr
                    if (allWords)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            rangePhrase += phrase[i];
                            //if not last
                            if (i < phrase.Length - 1)
                                rangePhrase += "-";
                        }
                        phraseTerm = new Term(rangePhrase, docNum);
                        addTermToTermList(phraseTerm);
                    }
                    //if one of the terms is a number its not the patern and we will check every term seperetly
                    else
                        for (int i = 0; i < 3; i++)
                            ParseTerm(phrase[i]);
                    break;
            }
        }


        /// <summary>
        /// checks if a given string is a number
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>true if a number, false otherwise</returns>
        private bool isTermNumber(string term)
        {
            double n;
            return Double.TryParse(term, out n);
        }

        /// <summary>
        /// check if the next term is part of the same stracture (price/ precent)
        /// </summary>
        /// <param name="term"> term </param>
        private void checkNextTerm(Term term)
        {
            if (index < TermsToSort.Length - 1)
            {
                string nextTerm = TermsToSort[index + 1];
                switch (nextTerm)
                {
                    case "dollars":
                    case "dollar":
                        term.Value = mergeTerms(term.Value, " dollars");
                        break;
                    case "u.s.":
                        if (index < TermsToSort.Length - 2)
                        {
                            if (TermsToSort[index + 2] == "dollars" || TermsToSort[index + 2] == "dollar")
                            {
                                term.Value = mergeTerms(term.Value, " dollars");
                                index++;
                            }
                        }
                        break;
                    case "precent":
                    case "percentage":
                        term.Value = mergeTerms(term.Value, "%");
                        break;
                    case "yuan":
                    case "yuans":
                        term.Value = mergeTerms(term.Value, " yuans");
                        break;
                    case "yen":
                    case "yens":
                        term.Value = mergeTerms(term.Value, " yens");
                        break;
                    case "ruble":
                    case "rubles":
                        term.Value = mergeTerms(term.Value, " rubles");
                        break;
                    case "peso":
                    case "pesos":
                        term.Value = mergeTerms(term.Value, " pesos");
                        break;
                }
            }
        }

        /// <summary>
        /// check if the word is a fraction
        /// </summary>
        /// <param name="term"></param>
        /// <returns>true if fraction </returns>
        private bool isWordFraction(string term)
        {
            if (term.Contains("/"))
            {
                string[] terms = term.Split('/');
                if (terms.Length == 2)
                {
                    double n;
                    return (double.TryParse(terms[0], out n) && double.TryParse(terms[1], out n));

                }
            }
            return false;
        }

        /// <summary>
        /// merge terms
        /// </summary>
        /// <param name="term1"> term1</param>
        /// <param name="term2"> term2</param>
        /// <returns>merged term</returns>
        private string mergeTerms(string term1, string term2)
        {
            term1 += term2;
            index++;
            return term1;
        }

        /// <summary>
        /// given a two digit number, check if it followd by a month- if so make it a date
        /// </summary>
        private void checkDateTerm(Term term)
        {
            if (index < TermsToSort.Length - 1)
            {
                string nextTerm = TermsToSort[index + 1];
                nextTerm = monthOfTerm(nextTerm);
                if (nextTerm != "0")
                    makeDateStartWithDay(nextTerm, term);
            }

        }

        /// <summary>
        /// make a date(when it starts with day followed a month), check for year
        /// </summary>
        /// <param name="month"> month</param>
        private void makeDateStartWithDay(string month, Term term)
        {
            string year = "";
            string day = term.Value;
            if (day.Length == 1) day = "0" + day;
            if (index < TermsToSort.Length - 2)
            {
                string nextword = TermsToSort[index + 2];
                int n;
                if (int.TryParse(nextword, out n))
                {
                    if (nextword.Length == 2)
                    {
                        if (Convert.ToInt32(nextword) > 50) year = 19 + nextword + "-";
                        else year = 20 + nextword + "-";
                        index++;
                    }
                    else if (nextword.Length == 4)
                    {
                        year = nextword + "-";
                        index++;
                    }
                }
            }
            index++;
            term.Value = (year + month + "-" + day);

        }
        /// <summary>
        /// for numbers over million , or numbers followed the words:million, billion, trillion-convert to millions and use the pattern (number M)
        /// </summary>
        /// <param name="term">number</param>
        private void ConvertToMillion(Term term)
        {
            bool millionAdded = false;
            double number = Convert.ToDouble(term.Value);
            if (index < TermsToSort.Length - 1)
            {
                string nextTerm = TermsToSort[index + 1];
                switch (nextTerm)
                {
                    case "million":
                    case "m":
                        term.Value = mergeTerms(term.Value, " m");
                        millionAdded = true;
                        break;
                    case "billion":
                    case "bn":
                        term.Value = mergeTerms(number * 1000 + " ", "m");
                        millionAdded = true;
                        break;
                    case "trillion":
                        term.Value = mergeTerms(number * 1000000 + " ", "m");
                        millionAdded = true;
                        break;
                }

            }
            //if million allready added no need to check
            if (!millionAdded)
            {
                if (number >= 1000000)
                {
                    number = number / 1000000;
                    term.Value = number + " m";
                }
            }
        }

        /// <summary>
        /// for numbers with the measure units: meter,cm,km,mm use pattern (number meters)
        /// for numbers with the measure units: kg,g use pattern (number kg)
        /// </summary>
        /// <param name="term">number</param>
        private void ConvertToMeasureUnits(Term term)
        {
            if (isTermNumber(term.Value.Replace(" m", "")))
            {
                double number;
                if (term.Value.Contains(" m"))
                {
                    number = Convert.ToDouble(term.Value.Replace(" m", "")) * 1000000;
                }
                else
                    number = Convert.ToDouble(term.Value);
                if (index < TermsToSort.Length - 1)
                {
                    string nextTerm = TermsToSort[index + 1];
                    switch (nextTerm)
                    {
                        case "centimeters":
                        case "cm":
                            number = number / 100;
                            if (number >= 1000000) term.Value = number / 1000000 + " m";
                            else term.Value = Convert.ToString(number);
                            term.Value = mergeTerms(term.Value, " meters");
                            break;
                        case "meters":
                        case "meter":
                            if (number >= 1000000) term.Value = number / 1000000 + " m";
                            else term.Value = Convert.ToString(number);
                            term.Value = mergeTerms(term.Value, " meters");
                            break;
                        case "kilometers":
                        case "kilometer":
                        case "km":
                            number = number * 1000;
                            if (number >= 1000000) term.Value = number / 1000000 + " m";
                            else term.Value = Convert.ToString(number);
                            term.Value = mergeTerms(term.Value, " meters");
                            break;
                        case "millimeters":
                        case "millimeter":
                        case "mm":
                            number = number / 1000;
                            if (number >= 1000000) term.Value = number / 1000000 + " m";
                            else term.Value = Convert.ToString(number);
                            term.Value = mergeTerms(term.Value, " meters");
                            break;
                        case "kilograms":
                        case "kilogram":
                        case "kg":
                            if (number >= 1000000) term.Value = number / 1000000 + " m";
                            else term.Value = Convert.ToString(number);
                            term.Value = mergeTerms(term.Value, " kg");
                            break;

                        case "grams":
                        case "gram":
                        case "g":
                            number = number / 1000;
                            if (number >= 1000000) term.Value = number / 1000000 + " m";
                            else term.Value = Convert.ToString(number);
                            term.Value = mergeTerms(term.Value, " kg");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// remove points and commas thats not a part of a number
        /// </summary>
        /// <param name="text">text </param>
        /// <returns>fixed text</returns>
        private string RemovePunctuationMarks(string text)
        {
            // remove `.` if not a part of number or (u.s.)
            char[] chars = text.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '.')
                {
                    if (i > 0 && i < chars.Length - 1)
                    {
                        if (chars[i - 1] == 'u' && chars[i + 1] == 's' && i < chars.Length - 2 && chars[i + 2] == '.')
                            i = i + 2;
                        else if (!Char.IsDigit(chars[i - 1]) || !Char.IsDigit(chars[i + 1]))
                            chars[i] = ' ';

                    }
                    else
                        chars[i] = ' ';

                }
                // remove `,` if not a part of number
                if (chars[i] == ',')
                {
                    if (i > 0 && i < chars.Length - 1)
                    {
                        if (!Char.IsDigit(chars[i - 1]) || !Char.IsDigit(chars[i + 1]))
                            chars[i] = ' ';
                    }
                    else
                        chars[i] = ' ';

                }
                // remove `/` if not a part of number
                if (chars[i] == '/')
                {
                    if (i > 0 && i < chars.Length - 1)
                    {
                        if (!Char.IsDigit(chars[i - 1]) || !Char.IsDigit(chars[i + 1]))
                            chars[i] = ' ';
                    }
                    else
                        chars[i] = ' ';
                }
            }
            text = new string(chars);
            text = text.Replace(",", "").Replace('[', ' ').Replace(']', ' ').Replace('(', ' ').Replace(')', ' ').Replace(';', ' ').Replace(':', ' ').Replace('{', ' ').Replace('}', ' ').Replace('!', ' ').Replace('?', ' ').Replace('|', ' ').Replace('#', ' ').Replace('"', ' ').Replace('<', ' ').Replace('>', ' ').Replace('*', ' ').Replace('+', ' ').Replace('=', ' ').Replace('`', ' ').Replace("�", "").Replace("\\", "").Replace('_', ' ').Replace('~', ' ').Replace('@', ' ');
            text = text.Replace("'s ", " ");
            text = text.Replace("'", "");
            return text;
        }
        /// <summary>
        /// for the last terms that was saved in the term list:
        /// 1) sand the list of terms to the indexer
        /// 2) marge the indexer to dictionery and post
        /// 3) save all the documents parsed
        /// </summary>
        public void SaveLastTerms()
        {
            if (terms.Count > 0)
            {
                indexer.Index(terms, pairs);
                terms.Clear();
                pairs.Clear();
            }
            indexer.MergeAll();
            SaveDocuments();
        }

        /// <summary>
        /// save the documents parsed to the disk
        /// </summary>
        private void SaveDocuments()
        {
            string path = IRSettings.Default.Destination;
            if (IRSettings.Default.Stemming)
                path += "\\Documents-WithStemming";
            else
                path += "\\Documents-WithoutStemming";
            File.Create(path).Close();
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (Document doc in l_documents)
                {
                    sw.WriteLine(doc.ToString());
                }
            }
        }
    }
}
