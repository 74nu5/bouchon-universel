namespace BouchonUniversel.Utils
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    #endregion

    public static class HttpUtils
    {
        public static Dictionary<string, IEnumerable<string>> GetHeadersFiltered(IHeaderDictionary headerDictionary)
            => headerDictionary
               .Where(pair => pair.Key != "Host")
               .ToDictionary(pair => pair.Key, pair => pair.Value.AsEnumerable());
    }
}
