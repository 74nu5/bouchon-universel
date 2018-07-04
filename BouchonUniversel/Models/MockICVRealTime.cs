using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using BouchonUniversel.Models.Bouchons;
using BouchonUniversel.Utils.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public static Object ResolveResponse (this string response, string contentType) {
            try {
                if (contentType == "application/json") {
                    return JsonConvert.DeserializeObject (response ?? "");
                } else if (contentType == "application/xml") {
                    return XDocument.Parse (response ?? "");
                }
            } catch (Exception e) {
                Console.WriteLine ("the response is not a valid json or xml");
            }
            return response;
        }

        /// <summary>Get the missing response in the list of reponses</summary>
        /// <param name="rootDir">The root directory</param>
        /// <param name="route">Root to get the response</param>
        /// <returns>The <see cref="string" />.</returns>
        /// <exception cref="Exception"></exception>        
        public static (string, ReponseBouchonnee) GetUpdatedResponse (string rootDir, string route) {

            var routes = route.Split ('/');
            var bouchonDir = new DirectoryInfo (Path.Combine (rootDir, string.Join ('/', routes.SkipLast (1))));
            var files = bouchonDir.GetFileSystemInfos ();
            string response = null;
            bool trouve = false;
            var i = 0;
            ReponseBouchonnee responseBouchon = new ReponseBouchonnee ();
            try {
                while (i < files.Length && !trouve) {
                    var dir = files[i];
                    if (dir is FileInfo) {
                        responseBouchon = (XDocument.Load (dir.FullName).FromXml<ReponseBouchonnee> ());
                        string body = responseBouchon.Body;
                        dynamic jObject = JsonConvert.DeserializeObject (body);
                        foreach (var d in jObject.data) {
                            if (d.id == routes.Last ().ToString ()) {
                                response = d.ToString ();
                            }
                        }
                    }
                    i++;
                }
            } catch (Exception e) {
                Console.WriteLine (e);
            }

            return (response, responseBouchon);
        }

    }
}