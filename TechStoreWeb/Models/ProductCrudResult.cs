namespace TechStoreWeb.Models;

public sealed class ProductCrudResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int? ProductID { get; set; }

    public bool BranchSyncSuccess { get; set; }

    public string? BranchSyncError { get; set; }
}
