namespace TechStoreWeb.Models;

public sealed class IntegrationSnapshot
{
    public bool LinkedServerHealthy { get; set; }

    public string? LinkedServerError { get; set; }

    public int HqProductCount { get; set; }

    public int BranchProductCount { get; set; }

    public int BranchInvoiceCount { get; set; }

    public int QueuePendingCount { get; set; }

    public int QueueSuccessCount { get; set; }

    public int QueueErrorCount { get; set; }
}
