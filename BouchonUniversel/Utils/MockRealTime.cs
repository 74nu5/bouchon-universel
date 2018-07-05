namespace BouchonUniversel.Utils
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    using BouchonUniversel.Models;
    using BouchonUniversel.Models.Bouchons;
    using BouchonUniversel.Utils.Xml;

    using Newtonsoft.Json;

    #endregion

    /// <summary>The mock icv real time.</summary>
    public static class MockRealTime
    {
        #region Méthodes publiques

        /// <summary>The get value.</summary>
        /// <param name="document">The document.</param>
        /// <param name="date">Date to Ajust.</param>
        /// <param name="patternsFormats">Pattern used.</param>
        /// <returns>The <see cref="string"/>.</returns>
        /// <exception cref="ArgumentException">Lève une exception lorsque la propriété et/ou la valeur n'est pas trouvé</exception>
        public static string AjustDates(this string document, string date, IEnumerable<PatternDateFormat> patternsFormats)
        {
            var ci = CultureInfo.InvariantCulture;
            var docResult = document;

            foreach (var pf in patternsFormats)
            {
                try
                {
                    var rgx = new Regex(pf.Pattern);

                    var matches = rgx.Matches(document);

                    if (!matches.Any())
                    {
                        continue;
                    }

                    foreach (var macthedDate in matches)
                    {
                        var separator = Regex.Match(macthedDate.ToString(), @"[/.-]").ToString();

                        var dateAjusted = AjustDate(date, macthedDate.ToString());

                        rgx = new Regex(@"-");
                        var test = rgx.Replace(pf.Format, separator);
                        docResult = docResult.Replace(macthedDate.ToString(), dateAjusted.ToString(test, ci));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return docResult;

            DateTime AjustDate(string dDateParam, string macthedDate)
            {
                var result = DateTime.Now;
                if (!DateTime.TryParse(macthedDate, out var dMacthedResult) || !DateTime.TryParse(dDateParam, out var dateParam))
                {
                    return result.ToUniversalTime();
                }

                var time = dateParam - dMacthedResult;
                result = dateParam.Add(time);

                return result.ToUniversalTime();
            }
        }

        /// <summary>Get the missing response in the list of reponses</summary>
        /// <param name="rootDir">The root directory</param>
        /// <param name="route">Root to get the response</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static (string, ReponseBouchonnee) GetUpdatedResponse(string rootDir, string route)
        {
            var routes = route.Split('/');
            var bouchonDir = new DirectoryInfo(Path.Combine(rootDir, string.Join('/', routes.SkipLast(1))));
            var files = bouchonDir.GetFileSystemInfos();
            string response = null;
            var trouve = false;
            var i = 0;
            var responseBouchon = new ReponseBouchonnee();
            try
            {
                while (i < files.Length && !trouve)
                {
                    var dir = files[i];
                    if (dir is FileInfo)
                    {
                        responseBouchon = XDocument.Load(dir.FullName).FromXml<ReponseBouchonnee>();
                        var body = responseBouchon.Body;
                        dynamic bodyObj = JsonConvert.DeserializeObject(body);
                        foreach (var d in bodyObj.data)
                        {
                            if (d.id != routes.Last())
                            {
                                continue;
                            }

                            response = d.ToString();
                            trouve = true;
                        }
                    }

                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return (response, responseBouchon);
        }

        /// <summary>The resolve response.</summary>
        /// <param name="response">The response.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public static object ResolveResponse(this string response, string contentType)
        {
            try
            {
                switch (contentType)
                {
                    case "application/json":
                        return JsonConvert.DeserializeObject(response ?? string.Empty);
                    case "application/xml":
                        return XDocument.Parse(response ?? string.Empty);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"the response is not a valid json or xml : {e.Message}");
            }

            return response;
        }

        #endregion
    }
}