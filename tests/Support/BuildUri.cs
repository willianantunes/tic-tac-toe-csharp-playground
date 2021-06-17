using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Tests.Support
{
    public class BuildUri
    {
        private readonly string _requestUri;
        private Dictionary<string, string> _queryParamsDictionary = new();

        public BuildUri(string requestUri)
        {
            _requestUri = requestUri;
        }

        public BuildUri AddParam(string key, object value)
        {
            // _queryParamsDictionary.Add(key, new[] { value.ToString() });
            _queryParamsDictionary.Add(key, value.ToString());
            return this;
        }

        public string Build()
        {
            var uriBuilder = new UriBuilder();
            uriBuilder.Path = _requestUri;
            var query = HttpUtility.ParseQueryString(string.Empty);

            foreach (KeyValuePair<string, string> entry in _queryParamsDictionary)
            {
                query[entry.Key] = entry.Value;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.PathAndQuery;
        }
    }
}
