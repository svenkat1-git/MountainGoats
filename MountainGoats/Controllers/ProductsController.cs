using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using MountainGoatsBikes.Repositories;

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

        // New Search GET action
        [HttpGet]
        public IActionResult Search()
        {
            var searchView = new ProductSearchViewModel();
            return View(searchView);
        }

        // New Search POST action
        [HttpPost]
        public IActionResult Search(ProductSearchViewModel searchView)
        {
            // Call the new method in the repository
            var searchResults = _repository.SearchProductDetails(
                searchView.BrandId,
                searchView.CategoryId,
                searchView.ProductName,
                searchView.StoreZip
            );

            // Attach results to the same view model
            searchView.Results = searchResults;

            // Return the same view with the results
            return View(searchView);
        }
    }
}
