using GoldenPagesUz.Models.YellowPages;

namespace GoldenPagesUz.Models;

public class Company
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Telephone { get; set; }
    public string YandexMapUrl { get; set; }
    public string Url { get; set; }

    public Company(ItemListElement itemListElement)
    {
        Name = itemListElement.Item?.Name ?? "Нет данных";
        Address = itemListElement.Item?.Address?.StreetAddress ?? "Нет данных";
        Telephone = itemListElement.Item?.Telephone ?? "Нет данных";

        if (itemListElement.Item?.Latitude == null || itemListElement.Item.Longitude == null)
            YandexMapUrl = "Нет данных";
        else 
            YandexMapUrl = $"https://yandex.com/maps/?pt={itemListElement.Item.Longitude.Trim()},{itemListElement.Item.Latitude.Trim()}&z=14&l=map";
        
        Url = itemListElement.Item?.SameAs ?? "Нет данных";
    }

    public Company(YpItem? ypItem)
    {
        Name = ypItem?.Name ?? "Нет данных";
        Address = ypItem?.Address?.StreetAddress ?? "Нет данных";
        Telephone = ypItem?.Telephone ?? "Нет данных";

        if (ypItem?.Geo?.Latitude == null || ypItem.Geo.Longitude == null)
            YandexMapUrl = "Нет данных";
        else 
            YandexMapUrl = $"https://yandex.com/maps/?pt={ypItem.Geo.Longitude.Trim()},{ypItem.Geo.Latitude.Trim()}&z=14&l=map";

        Url = ypItem?.SameAs ?? "Нет данных";
    }
}