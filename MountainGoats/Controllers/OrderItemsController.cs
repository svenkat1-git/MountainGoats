using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Repositories;
using Microsoft.Extensions.Configuration;
using System;

namespace MountainGoatsBikes.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly Repository _repository;

        public OrderItemsController(Repository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var orderItems = _repository.GetOrderItems();
            return View(orderItems);
        }
    }
}

