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
    }
}
