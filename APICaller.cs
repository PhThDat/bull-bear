using System.Threading.Tasks;
using System;

class APICaller
{
    public static async Task<string> GetMsg(string url)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else return null;
        }
    }
}