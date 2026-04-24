using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechStoreWeb.Models;
using TechStoreWeb.Services;

namespace TechStoreWeb.Pages;

public class IndexModel : PageModel
{
    private readonly TechStoreRepository _repository;

    public IndexModel(TechStoreRepository repository)
    {
        _repository = repository;
    }

    [TempData]
    public string? StatusMessage { get; set; }

    public IntegrationSnapshot Snapshot { get; private set; } = new();

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync()
    {
        try
        {
            Snapshot = await _repository.GetIntegrationSnapshotAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
