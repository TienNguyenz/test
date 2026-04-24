using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TechStoreWeb.Models;
using TechStoreWeb.Services;

namespace TechStoreWeb.Pages.Products;

public class UpdatePriceModel : PageModel
{
    private readonly TechStoreRepository _repository;

    public UpdatePriceModel(TechStoreRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    [Range(typeof(decimal), "1000", "999999999", ErrorMessage = "Price must be from 1,000 to 999,999,999.")]
    public decimal NewPrice { get; set; }

    public Product? Product { get; private set; }

    public UpdatePriceResult? SyncResult { get; private set; }

    public IReadOnlyList<PriceSyncQueueItem> RecentQueue { get; private set; } = [];

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            if (Id <= 0)
            {
                var products = await _repository.GetProductsAsync();
                Id = products.FirstOrDefault()?.ProductID ?? 0;
            }

            if (Id <= 0)
            {
                ErrorMessage = "Khong tim thay san pham de cap nhat.";
                return Page();
            }

            Product = await _repository.GetProductByIdAsync(Id);
            if (Product is null)
            {
                ErrorMessage = "ProductID khong ton tai.";
                return Page();
            }

            NewPrice = Product.Price;
            RecentQueue = await _repository.GetLatestQueueAsync();
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (Id <= 0)
            {
                ModelState.AddModelError(string.Empty, "ProductID khong hop le.");
            }

            Product = await _repository.GetProductByIdAsync(Id);
            if (Product is null)
            {
                ModelState.AddModelError(string.Empty, "Khong tim thay san pham.");
            }

            if (!ModelState.IsValid)
            {
                RecentQueue = await _repository.GetLatestQueueAsync();
                return Page();
            }

            SyncResult = await _repository.UpdatePriceAndSyncAsync(Id, NewPrice);
            Product = await _repository.GetProductByIdAsync(Id);
            RecentQueue = await _repository.GetLatestQueueAsync();
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            RecentQueue = await _repository.GetLatestQueueAsync();
            return Page();
        }
    }
}
