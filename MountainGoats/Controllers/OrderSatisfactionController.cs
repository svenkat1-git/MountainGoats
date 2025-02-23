using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Repositories;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
    public class OrderSatisfactionController : Controller
    {
        private readonly Repository _repository;

        public OrderSatisfactionController(Repository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var satisfactions = _repository.GetOrderSatisfactions();
            return View(satisfactions);
        }
    }
}

