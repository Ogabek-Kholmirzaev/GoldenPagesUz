using Newtonsoft.Json;

namespace GoldenPagesUz.Models;

public class ItemListElement
{
    [JsonProperty("@type")]
    public string Type { get; set; }

    public int Position { get; set; }
    public Item Item { get; set; }
}

public class Item
{
    [JsonProperty("@type")]
    public string Type { get; set; }

    [JsonProperty("@context")]
    public string COntext { get; set; }

    public string Name { get; set; }
    public Address Address { get; set; }
    public string SameAs { get; set; }
    public string Email { get; set; }
    public string FaxNumber { get; set; }
    public string Telephone { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}

public class Address
{
    [JsonProperty("@type")]
    public string Type { get; set; }

    public int PostalCode { get; set; }
    public string StreetAddress { get; set; }
}