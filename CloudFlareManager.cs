using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShareMe 
{
    public static class CloudFlareManager
    {
        private const string CF_API_BASE_URL = "https://api.cloudflare.com/client/v4";

        public static async Task PurgeCache(string zone, string email, string key, params string[] uris)
        {
            if (!string.IsNullOrWhiteSpace(zone) &&
                !string.IsNullOrWhiteSpace(email) &&
                !string.IsNullOrWhiteSpace(key) &&
                uris.Length > 0)
            {
                Console.WriteLine($"Purging ['{string.Join("', '", uris)}'] from cloudflare cache");

                using(HttpClient client = new HttpClient())
                {
                    var content = new StringContent(new 
                    {
                        files = uris
                    }.ToString(), Encoding.UTF8, "application/json");
                    
                    client.DefaultRequestHeaders.Add("X-Auth-Email", email);
                    client.DefaultRequestHeaders.Add("X-Auth-Key", key);

                    client.BaseAddress = new Uri(CF_API_BASE_URL);
                    var result = await client.PostAsync($"/zones/{zone}/purge_cache", content);

                    Console.WriteLine(await result.Content.ReadAsStringAsync());
                }
            }
        }
    }
}