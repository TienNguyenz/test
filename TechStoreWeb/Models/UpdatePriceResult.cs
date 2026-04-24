namespace TechStoreWeb.Models;

public sealed class UpdatePriceResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public decimal? BranchPriceAfterSync { get; set; }

    public PriceSyncQueueItem? LatestQueueItem { get; set; }
}
