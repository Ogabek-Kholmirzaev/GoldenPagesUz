using Newtonsoft.Json;

namespace GoldenPagesUz.Models.YellowPages;

public class YpJsonParser<T> where T : class
{
    [JsonProperty("@context")]
    public string? Context { get; set; }

    [JsonProperty("@type")]
    public string? Type { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    
    public AggregateRating? AggregateRating { get; set; }
    public T? MainEntity { get; set; }
}

public class AggregateRating
{
    [JsonProperty("@type")]
    public string? Type { get; set; }

    public decimal BestRating { get; set; }
    public decimal RatingValue { get; set; }
    public decimal WorstRating { get; set; }
    public decimal ReviewCount { get; set; }
}

public class MainEntityV1
{
    [JsonProperty("@type")]
    public string? Type { get; set; }

    public List<YpItemListElement>? ItemListElement { get; set; }
}

public class MainEntityV2
{
    [JsonProperty("@type")]
    public string? Type { get; set; }

    public YpItemListElement? ItemListElement { get; set; }
}

public class YpItemListElement
{
    [JsonProperty("@type")]
    public string? Type { get; set; }

    public YpItem? Item { get; set; }
    public int Position { get; set; }
}

public class YpItem
{
    [JsonProperty("@type")]
    public string? Type { get; set; }

    public string? Name { get; set; }
    public string? SameAs { get; set; }
    public YpAddress? Address { get; set; }
    public string? Email { get; set; }
    public YpGeo? Geo { get; set; }
    public string? Logo { get; set; }
    public string? Telephone { get; set; }
}

public class YpAddress
{
    [JsonProperty("@type")]
    public string? Type { get; set; }

    public string? StreetAddress { get; set; }
}

public class YpGeo
{
    [JsonProperty("@type")]
    public string? Type { get; set; }

    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
}