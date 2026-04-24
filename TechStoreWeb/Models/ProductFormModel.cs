using System.ComponentModel.DataAnnotations;

namespace TechStoreWeb.Models;

public sealed class ProductFormModel
{
    [Display(Name = "Product Name")]
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be 2-200 characters.")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Price (VND)")]
    [Range(typeof(decimal), "1000", "999999999", ErrorMessage = "Price must be from 1,000 to 999,999,999.")]
    public decimal Price { get; set; }

    [Display(Name = "Category")]
    [Required(ErrorMessage = "Category is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Category must be 2-100 characters.")]
    public string Category { get; set; } = string.Empty;
}
