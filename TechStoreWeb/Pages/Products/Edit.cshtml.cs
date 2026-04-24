using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechStoreWeb.Models;
using TechStoreWeb.Services;

namespace TechStoreWeb.Pages.Products;

public class EditModel : PageModel
{
    private readonly TechStoreRepository _repository;

    public EditModel(TechStoreRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public ProductFormModel Input { get; set; } = new();

    public Product? CurrentProduct { get; private set; }

    public ProductCrudResult? Result { get; private set; }

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (Id <= 0)
        {
            return RedirectToPage("/Products/Index");
        }

        CurrentProduct = await _repository.GetProductByIdAsync(Id);
        if (CurrentProduct is null)
        {
            ErrorMessage = "Product not found.";
            return Page();
        }

        Input = new ProductFormModel
        {
            ProductName = CurrentProduct.ProductName,
            Price = CurrentProduct.Price,
            Category = CurrentProduct.Category
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Id <= 0)
        {
            return RedirectToPage("/Products/Index");
        }

        CurrentProduct = await _repository.GetProductByIdAsync(Id);
        if (CurrentProduct is null)
        {
            ErrorMessage = "Product not found.";
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Result = await _repository.UpdateProductAsync(Id, Input);

        CurrentProduct = await _repository.GetProductByIdAsync(Id);

        if (Result.Success)
        {
            TempData["StatusMessage"] = Result.Message;
        }
        else
        {
            ModelState.AddModelError(string.Empty, Result.Message);
        }

        return Page();
    }
}
