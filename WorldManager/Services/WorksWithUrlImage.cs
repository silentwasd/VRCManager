using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WorldManager.Services;

public static class WorksWithUrlImage
{
    public static async Task<Stream> LoadImageFromUrl(string url, string filename, Task<Stream> defaultImage)
    {
        if (File.Exists("images/" + filename))
            return new MemoryStream(await File.ReadAllBytesAsync("images/" + filename));

        var response = AppCompose.HttpClient.GetAsync(url);

        if (response.Result.StatusCode != HttpStatusCode.OK) return await defaultImage;

        var data = await response.Result.Content.ReadAsByteArrayAsync();

        if (!Directory.Exists("images"))
            Directory.CreateDirectory("images");

        await File.WriteAllBytesAsync("images/" + filename, data);

        return new MemoryStream(data);
    }
}