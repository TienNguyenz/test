using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechStoreWeb.Models;
using TechStoreWeb.Services;

namespace TechStoreWeb.Pages.Products;

public class CreateModel : PageModel
{
    private readonly TechStoreRepository _repository;

    public CreateModel(TechStoreRepository repository)
    {
        _repository = repository;
    }

    [BindProperty]
    public ProductFormModel Input { get; set; } = new();

    public ProductCrudResult? Result { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Result = await _repository.CreateProductAsync(Input);

        if (Result.Success)
        {
            TempData["StatusMessage"] = Result.Message;
            return RedirectToPage("/Products/Edit", new { id = Result.ProductID });
        }

        ModelState.AddModelError(string.Empty, Result.Message);
        return Page();
    }
}
