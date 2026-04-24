using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechStoreWeb.Models;
using TechStoreWeb.Services;

namespace TechStoreWeb.Pages.Distributed;

public class EvidenceModel : PageModel
{
    private readonly TechStoreRepository _repository;

    public EvidenceModel(TechStoreRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    public IntegrationSnapshot Snapshot { get; private set; } = new();

    public IReadOnlyList<TransparentJoinRow> JoinRows { get; private set; } = [];

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync()
    {
        try
        {
            Snapshot = await _repository.GetIntegrationSnapshotAsync();
            JoinRows = await _repository.GetTransparentJoinRowsAsync(FromDate, ToDate, take: 200);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
