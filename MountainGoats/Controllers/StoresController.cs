using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MountainGoatsBikes.Repositories;

namespace MountainGoatsBikes.Controllers
{
    public class StoresController : Controller
    {
        private readonly Repository _repository;

        public StoresController(Repository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var stores = _repository.GetStores();
            return View(stores);
        }
    }
}
