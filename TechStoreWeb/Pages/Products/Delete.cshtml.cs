using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechStoreWeb.Models;
using TechStoreWeb.Services;

namespace TechStoreWeb.Pages.Products;

public class DeleteModel : PageModel
{
    private readonly TechStoreRepository _repository;

    public DeleteModel(TechStoreRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public Product? Product { get; private set; }

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (Id <= 0)
        {
            return RedirectToPage("/Products/Index");
        }

        Product = await _repository.GetProductByIdAsync(Id);
        if (Product is null)
        {
            ErrorMessage = "Product not found.";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Id <= 0)
        {
            return RedirectToPage("/Products/Index");
        }

        var result = await _repository.DeleteProductAsync(Id);
        TempData["StatusMessage"] = result.Message;
        return RedirectToPage("/Products/Index");
    }
}
