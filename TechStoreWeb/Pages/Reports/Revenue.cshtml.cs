using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechStoreWeb.Models;
using TechStoreWeb.Services;

namespace TechStoreWeb.Pages.Reports;

public class RevenueModel : PageModel
{
    private readonly TechStoreRepository _repository;

    public RevenueModel(TechStoreRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    public IReadOnlyList<RevenueRow> Rows { get; private set; } = [];

    public decimal GrandRevenue => Rows.Sum(x => x.DoanhThu);

    public int TotalUnits => Rows.Sum(x => x.SoLuongBan);

    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync()
    {
        try
        {
            Rows = await _repository.GetRevenueAsync(FromDate, ToDate);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
