using Microsoft.AspNetCore.Mvc.RazorPages;
using TechStoreWeb.Models;
using TechStoreWeb.Services;

namespace TechStoreWeb.Pages.Invoices;

public class IndexModel : PageModel
{
    private readonly TechStoreRepository _repository;

    public IndexModel(TechStoreRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<Invoice> Invoices { get; private set; } = [];

    public decimal TotalRevenue => Invoices.Sum(x => x.Total);

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync()
    {
        try
        {
            Invoices = await _repository.GetInvoicesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
