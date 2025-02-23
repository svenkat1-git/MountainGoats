using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MountainGoatsBikes.Repositories;

namespace MountainGoatsBikes.Controllers
{
    public class StocksController : Controller
    {
        private readonly Repository _repository;

        public StocksController(Repository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var stocks = _repository.GetStocks();
            return View(stocks);
        }
    }
}
