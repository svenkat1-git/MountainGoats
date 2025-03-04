using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MountainGoatsBikes.Models;
using MountainGoatsBikes.Repositories;
using System.Linq;

namespace MountainGoatsBikes.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Repository _repository;

        public ProductsController(Repository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var products = _repository.GetProducts();
            return View(products);
        }

        // GET: /Products/Search
        [HttpGet]
        public IActionResult Search()
        {
            var vm = new ProductSearchViewModel();
            PopulateDropdowns(vm);
            return View(vm);
        }

        // POST: /Products/Search
        [HttpPost]
        public IActionResult Search(ProductSearchViewModel vm)
        {
            // Execute the search using the repository method
            var searchResults = _repository.SearchProductDetails(
                vm.BrandId,
                vm.CategoryId,
                vm.ProductName,
                vm.StoreZip
            );
            vm.Results = searchResults;
            // Repopulate the dropdowns so that the view retains them
            PopulateDropdowns(vm);
            return View(vm);
        }

        private void PopulateDropdowns(ProductSearchViewModel vm)
        {
            // Populate brands from the database
            var brands = _repository.GetBrands();
            vm.Brands = brands.Select(b => new SelectListItem
            {
                Value = b.BrandId.ToString(),
                Text = b.BrandName
            });

            // Populate categories from the database
            var categories = _repository.GetCategories();
            vm.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            });

            // Populate distinct store zip codes from the stores table
            var stores = _repository.GetStores();
            var zips = stores
                .Where(s => !string.IsNullOrEmpty(s.ZipCode))
                .Select(s => s.ZipCode)
                .Distinct();
            vm.StoreZips = zips.Select(z => new SelectListItem
            {
                Value = z,
                Text = z
            });
        }
    }
}
