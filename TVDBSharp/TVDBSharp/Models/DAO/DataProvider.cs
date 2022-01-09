using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TVDBSharp.Models.Deserialization;
using TVDBSharp.Utilities;

namespace TVDBSharp.Models.DAO
{
    /// <summary>
    ///     Standard implementation of the <see cref="IDataProvider" /> interface.
    /// </summary>
    public class DataProvider : IDataProvider
    {
        private const string BaseUrl = "https://api.thetvdb.com";
        private readonly string _apiKey;
        private string _authToken;
        
        public DataProvider(string apiKey)
        {
            _apiKey = apiKey;
        }

        private async Task<string> GetJwtToken(string apiKey)
        {
            //return
            //    "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJhZ2UiOiIiLCJhcGlrZXkiOiIiLCJjb21tdW5pdHlfc3VwcG9ydGVkIjpmYWxzZSwiZXhwIjoxNjQzNzUxNzE4LCJnZW5kZXIiOiIiLCJpZCI6IjIzNjQ3ODYiLCJpc19tb2QiOmZhbHNlLCJpc19zeXN0ZW1fa2V5IjpmYWxzZSwiaXNfdHJ1c3RlZCI6ZmFsc2UsInBpbiI6bnVsbCwidXVpZCI6IiJ9.mJsTEy8RWH4CkTlHBFtyuGschv__v9ICI2x2qq4lEa8HLlA3WJSkDuUPBRCUMMC3MyLfXQ8Tp5msETdI0P7AUCMRsfYUXVJvjlULAowswC_MeUgxQ46VSjPFcXSN7tC9Dv6_8vry9l_ESCu_5F9jQt0xvyRK2dnBjP57TqGw31Ufp7rqUVfbX-SMp3hLhsFOvCeYzmuBjWVWyh3yZm2iXkmToju9H7RsEozO8OjrURElUPTxuc_wZjq1OAxtnXSkO3H6GaiD-l1dhZsT4ep-yF1xYdMqCl8fctuXORKe02nCcq9zWEVa0BRlyVpQVmr5beeOash-MKsAa800WwGHAGbT-VEFcFeKZXB5v4QL21WmkJgqxOKkOBQdzMy4YeTSZN7eLQwJ1bJfDFanAwVaFCBDJDdRravZZqTT-FlwJKYrCpDc3sFj8lRGb8nZ59jOQr7sfAsbgATJV0rPE4If18AmuSj8eC1nmWO59nwT-uvWDXLjNU6eSco4V7iWwHtGrKo0fGytsIMNzsLQUQX5eXFyPO4fC5d4cSJBX69Y5KDT2WYc-FXqhifpGjerVQEVxDxR2P-sLYw99rhx9IGPeOK5k398rbKfUEBKgeFzFv8570hJA5a4pQ0nTRbIY_ZbKivtqIxuIVvF08TCc7YMZsgd2lZinnsFjAr7970-hOc";


            using (var client = new HttpClient()) 
            {
                var response = await client.PostAsync($"{BaseUrl}/login", new StringContent($"{{ \"apikey\": \"{apiKey}\" }}", Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                dynamic item = JsonConvert.DeserializeObject<object>(content);
                var token = item["token"];
                return token;
            }

            throw new InvalidOperationException("Unable to retrieve the authentication token");
        }

        public async Task<Show> GetShow(int showID) => await GetResponse<Show>($"{BaseUrl}/series/{showID}");

        public async Task<List<Episode>> GetEpisodes(int showId, int page) => await GetResponse<List<Episode>>($"{BaseUrl}/series/{showId}/episodes?page={page}");

        public async Task<List<UpdateTimestamp>> GetUpdates(DateTime from, DateTime to) => await GetResponse<List<UpdateTimestamp>>($"{BaseUrl}/updated/query?fromTime={from.ToEpoch()}&toTime={to.ToEpoch()}");

        public async Task<List<Show>> Search(string query) => await GetResponse<List<Show>>($"{BaseUrl}/search/series?name={query}");

        private async Task<T> GetResponse<T>(string url)
        {
            if (string.IsNullOrWhiteSpace(_authToken))
            {
                _authToken = await GetJwtToken(_apiKey);
            }

            using (var web = new HttpClient())
            {
                web.DefaultRequestHeaders.Add("Authorization", $"Bearer {_authToken}");
                web.DefaultRequestHeaders.Add("Accept", "application/vnd.thetvdb.v3");

                var json = await web.GetAsync(url);
                var content = await json.Content.ReadAsStringAsync();

                var root = JsonConvert.DeserializeObject<Root<T>>(content);
                return root.Data;
            }
        }
    }
}
