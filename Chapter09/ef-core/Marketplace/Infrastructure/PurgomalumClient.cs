using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace Marketplace.Infrastructure
{
    /// <summary>
    /// PurgoMalum is a simple, free, RESTful web service for filtering and removing content of profanity, obscenity and other unwanted text.
    /// Check http://www.purgomalum.com
    /// </summary>
    public class PurgomalumClient
    {
        private readonly HttpClient _httpClient;

        public PurgomalumClient() : this(new HttpClient()) { }
        
        public PurgomalumClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<bool> CheckForProfanity(string text)
        {
            var result = await _httpClient.GetAsync(
                QueryHelpers.AddQueryString("https://www.purgomalum.com/service/containsprofanity", "text", text));
            
            var value = await result.Content.ReadAsStringAsync();
            return bool.Parse(value);
        }
    }
}