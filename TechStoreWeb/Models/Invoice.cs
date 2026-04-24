namespace TechStoreWeb.Models;

public sealed class Invoice
{
    public long InvoiceID { get; set; }

    public int ProductID { get; set; }

    public int Quantity { get; set; }

    public DateTime SaleDate { get; set; }

    public decimal Total { get; set; }
}
