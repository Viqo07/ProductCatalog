using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Models;
using System.Text.Json;

namespace ProductCatalog.Controllers;

public class ProductsController : Controller
{
    private readonly string _filePath;

    public ProductsController(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(
            environment.ContentRootPath,
            "data",
            "products.json"
        );
    }

    public IActionResult Index()
    {
        List<Product> products = LoadProducts();
        return View(products);
    }

    public IActionResult Details(int id)
    {
        List<Product> products = LoadProducts();
        Product? product = products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        List<Product> products = LoadProducts();

        product.Id = products.Count == 0
            ? 1
            : products.Max(p => p.Id) + 1;

        products.Add(product);
        SaveProducts(products);

        return RedirectToAction(nameof(Index));
    }

    private List<Product> LoadProducts()
    {
        if (!System.IO.File.Exists(_filePath))
        {
            return new List<Product>();
        }

        string json = System.IO.File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Product>();
        }

        return JsonSerializer.Deserialize<List<Product>>(json)
            ?? new List<Product>();
    }

    private void SaveProducts(List<Product> products)
    {
        string json = JsonSerializer.Serialize(
            products,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        System.IO.File.WriteAllText(_filePath, json);
    }
}
