using System.Collections.Generic;
using Newtonsoft.Json;

namespace WorldManager.Models.VrcApi;

public class World
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string AuthorId { get; set; }

    public string AuthorName { get; set; }

    public string ThumbnailImageUrl { get; set; }

    public string ImageUrl { get; set; }

    public int Popularity { get; set; }

    public int Visits { get; set; }

    public int Favorites { get; set; }

    public int Heat { get; set; }

    public int Capacity { get; set; }

    public string Description { get; set; }

    public List<string> Tags { get; set; }

    public string PreviewYoutubeId { get; set; }

    public string PublicationDate { get; set; }

    [JsonProperty("updated_at")] public string UpdatedAt { get; set; }
}