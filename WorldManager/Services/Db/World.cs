using VRChat.API.Model;

namespace WorldManager.Services.Db;

public class World
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string AuthorId { get; set; }

    public string AuthorName { get; set; }

    public string ThumbnailImageUrl { get; set; }

    public string ImageUrl { get; set; }

    public string? Group { get; set; }

    public string? Note { get; set; }

    public static World FromLimited(LimitedWorld source)
    {
        return new World
        {
            Id = source.Id,
            Name = source.Name,
            AuthorId = source.AuthorId,
            AuthorName = source.AuthorName,
            ThumbnailImageUrl = source.ThumbnailImageUrl,
            ImageUrl = source.ImageUrl
        };
    }

    public static World FromFull(Models.VrcApi.World source)
    {
        return new World
        {
            Id = source.Id,
            Name = source.Name,
            AuthorId = source.AuthorId,
            AuthorName = source.AuthorName,
            ThumbnailImageUrl = source.ThumbnailImageUrl,
            ImageUrl = source.ImageUrl
        };
    }
}