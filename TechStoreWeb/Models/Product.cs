namespace TechStoreWeb.Models;

public sealed class Product
{
    public int ProductID { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;
}
