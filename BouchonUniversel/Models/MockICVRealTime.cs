using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BouchonUniversel.Models {

    public static class MockICVRealTime {
        /// <summary>The get value.</summary>
        /// <param name="dDate">Date to Ajust.</param>
        /// <param name="patternsFormats">Pattern used.</param>
        /// <returns>The <see cref="string" />.</returns>
        /// <exception cref="ArgumentException">Lève une exception lorsque la propriété et/ou la valeur n'est pas trouvé</exception>
        public static string AjustDates (this string document, string dDate, List<PatternDateFormat> patternsFormats) {
            CultureInfo ci = CultureInfo.InvariantCulture;
            MatchCollection mactches = null;
            string docResult = document;;

            foreach (var pf in patternsFormats) {
                try {
                    Regex rgx = new Regex (pf.pattern);

                    mactches = rgx.Matches (document);

                    if (mactches.Count > 0) {
                        foreach (var macthedDate in mactches) {

                            string separator = Regex.Match (macthedDate.ToString (), @"[/.-]").ToString ();

                            DateTime dateAjusted = AjustDate (dDate, macthedDate.ToString ());

                            rgx = new Regex (@"-");
                            string test = rgx.Replace (pf.format, separator);
                            docResult = docResult.Replace (macthedDate.ToString (), dateAjusted.ToString (test, ci));
                        }
                    }
                } catch (Exception e) {
                    Console.WriteLine (e);
                }

            }

            return docResult;

            DateTime AjustDate (string dDateParam, string macthedDate) {
                DateTime dMacthedResult;
                DateTime dateParam;
                DateTime dResult = DateTime.Now;
                if (DateTime.TryParse (macthedDate, out dMacthedResult) && DateTime.TryParse (dDateParam, out dateParam)) {
                    TimeSpan time = dateParam - dMacthedResult;
                    dResult = dateParam.Add (time);
                }

                return dResult.ToUniversalTime ();
            }
        }
    }
}