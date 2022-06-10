using Newtonsoft.Json.Linq;

namespace WorldManager.Models;

public class ApiError
{
    public string Message { get; set; }

    public string StatusCode { get; set; }

    public static ApiError Parse(object content)
    {
        var strContent = (string)content;
        var json = JObject.Parse(strContent);
        return new ApiError
        {
            Message = (string)json["error"]["message"],
            StatusCode = (string)json["error"]["status_code"]
        };
    }
}