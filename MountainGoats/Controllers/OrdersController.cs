using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Repositories;
using Microsoft.Extensions.Configuration;
using System;

namespace MountainGoatsBikes.Controllers
{
    public class OrdersController : Controller
    {
        private readonly Repository _repository;

        public OrdersController(Repository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var orders = _repository.GetOrders();
            return View(orders);
        }
    }
}

