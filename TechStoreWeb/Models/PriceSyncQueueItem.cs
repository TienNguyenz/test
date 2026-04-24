namespace TechStoreWeb.Models;

public sealed class PriceSyncQueueItem
{
    public long QueueID { get; set; }

    public int ProductID { get; set; }

    public decimal NewPrice { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public string? LastError { get; set; }
}
