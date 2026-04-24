namespace TechStoreWeb.Models;

public sealed class TransparentJoinRow
{
    public int ProductID { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal HQPrice { get; set; }

    public int Quantity { get; set; }

    public DateTime SaleDate { get; set; }

    public decimal BranchTotal { get; set; }

    public decimal RevenueByHQPrice { get; set; }
}
