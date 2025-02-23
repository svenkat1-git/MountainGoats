using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MountainGoatsBikes.Repositories;

namespace MountainGoatsBikes.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly Repository _repository;
        public CategoriesController(Repository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var categories = _repository.GetCategories();
            return View(categories);
        }
    }
}