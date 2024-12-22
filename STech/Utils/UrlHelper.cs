using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Specialized;
using System.Web;

namespace STech.Utils
{
    public static class UrlHelper
    {
        public static string? RemoveHash(this string? url)
        {
            int hashIndex = url?.IndexOf('#') ?? 0;
            if (hashIndex >= 0)
            {
                return url?.Substring(0, hashIndex);
            }

            return url;
        }

        public static string AddOrUpdateQueryParam(string currentUrl, string paramName, string paramValue)
        {
            UriBuilder uriBuilder = new UriBuilder(currentUrl);
            NameValueCollection queryParams = HttpUtility.ParseQueryString(uriBuilder.Query);

            queryParams.Set(paramName, paramValue);
            uriBuilder.Query = queryParams.ToString();

            return uriBuilder.ToString();
        }

        public static string AddOrUpdateQueryParams(string currentUrl, NameValueCollection updatedParams)
        {
            UriBuilder uriBuilder = new UriBuilder(currentUrl);
            NameValueCollection queryParams = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (string key in updatedParams)
            {
                queryParams.Set(key, updatedParams[key]);
            }

            uriBuilder.Query = queryParams.ToString();

            return uriBuilder.ToString();
        }
    }
}
