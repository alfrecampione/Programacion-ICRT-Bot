using System.Net.Http.Json;

namespace DLL;

public abstract class HttpConnection<T> where T : ResponseBase
{
    public static async Task<T?> GetResponseAsync(string path)
    {
        var client = new HttpClient();
        var responseMessage = await client.GetAsync(path);
        if (!responseMessage.IsSuccessStatusCode) return null;
        var response = await responseMessage.Content.ReadFromJsonAsync<T>();
        return response;
    }
}