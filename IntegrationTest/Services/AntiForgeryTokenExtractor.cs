using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IntegrationTest.Services
{
    /// <summary>
    /// The <seealso cref="AntiForgeryTokenExtractor"/> is mostly extracted from <see cref=" https://github.com/yogyogi/ASP.NET-Core-Unit-Testing-with-xUnit.git"/>. 
    /// </summary>
    public static class AntiForgeryTokenExtractor
    {
        public static string Field { get; } = "__RequestVerificationToken"; // *** Has to be match with your Token. It may be different on different version of dotnet. 
        public static string Cookie { get; } = ".AspNetCore.Antiforgery.oUPZnAolfCc"; // AntiForgeryTokenCookie


        public static string ExtractAntiForgeryToken(string htmlBody)
        {
            var requestVerificationTokenMatch = Regex.Match(htmlBody, $@"\<input name=""{Field}"" type=""hidden"" value=""([^""]+)"" \/\>");
            if (requestVerificationTokenMatch.Success)
                return requestVerificationTokenMatch.Groups[1].Captures[0].Value;
            throw new ArgumentException($"Anti forgery token '{Field}' not found", nameof(htmlBody));
        }

        /// <summary>
        /// Cookie Extracting Method has been change. This is created by me by looking at the Http Header Values in debugging view. 
        /// </summary>
        /// <param name="httpHeader"></param>
        /// <returns></returns>
        public static string ExtractCookieValue(string httpHeader)
        {
            var splitHttpheader = httpHeader.Split(';');
            int position = splitHttpheader[0].IndexOf('=');
            
            return splitHttpheader[0].Substring(position + 1);
        }
        public static async Task<(string field, string cookie)> ExtractAntiForgeryValues(string httpHeader, string responseContent)
        {
            var cookie = ExtractCookieValue(httpHeader); 
            var token = ExtractAntiForgeryToken(responseContent);

            return (token, cookie);
        }


    }// end class AntiForgeryTokenExtractor
}
