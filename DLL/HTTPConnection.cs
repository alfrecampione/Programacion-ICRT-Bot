using System.Net.Http.Json;
namespace DLL;

public class HttpConnection<T> where T:ResponseBase
{
    public static async Task<T?> GetResponseAsync(string path)
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage responseMessage = await client.GetAsync(path);
        if (responseMessage.IsSuccessStatusCode)
        {
            var response =  await responseMessage.Content.ReadFromJsonAsync<T>();
            return response;
        }
        return null;
    }
}