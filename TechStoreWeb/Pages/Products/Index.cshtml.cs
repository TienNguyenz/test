using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechStoreWeb.Models;
using TechStoreWeb.Services;

namespace TechStoreWeb.Pages.Products;

public class IndexModel : PageModel
{
    private readonly TechStoreRepository _repository;

    public IndexModel(TechStoreRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<Product> Products { get; private set; } = [];

    public string? ErrorMessage { get; private set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            Products = await _repository.GetProductsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
