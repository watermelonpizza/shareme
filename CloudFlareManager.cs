using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                    var content = new StringContent(
                        JsonConvert.SerializeObject(
                            new 
                            {
                                files = uris
                            }),
                        Encoding.UTF8,
                        "application/json");

                    client.BaseAddress = new Uri(CF_API_BASE_URL);

                    HttpRequestMessage request = 
                        new HttpRequestMessage(
                            HttpMethod.Delete,
                            $"/zones/{zone}/purge_cache");

                    request.Headers.Add("X-Auth-Email", email);
                    request.Headers.Add("X-Auth-Key", key);
                    request.Content = content;

                    HttpResponseMessage result = null;
                    try
                    {
                        result = await client.SendAsync(request);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    Console.WriteLine(await result?.Content.ReadAsStringAsync());
                }
            }
        }
    }
}