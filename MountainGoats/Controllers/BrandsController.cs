using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Repositories;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
    public class BrandsController : Controller
    {
        private readonly Repository _repository;
        public BrandsController(Repository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var brands = _repository.GetBrands();
            return View(brands);
        }
    }
}