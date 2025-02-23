using Microsoft.AspNetCore.Mvc;
using MountainGoatsBikes.Repositories;
using MountainGoatsBikes.Models;
using System.Linq;
using System.Collections.Generic;

namespace MountainGoatsBikes.Controllers
{
    public class CustomersController : Controller
    {
        private readonly Repository _repository;

        public CustomersController(Repository repository)
        {
            _repository = repository;
        }

        // Index action with paging (10 customers per page)
        public IActionResult Index(int page = 1)
        {
            const int pageSize = 10;
            int offset = (page - 1) * pageSize;
            int totalRecords = _repository.GetTotalCustomerCount();
            var customers = _repository.GetCustomersPaged(offset, pageSize).ToList();

            var model = new CustomerList
            {
                Customers = customers,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return View(model);
        }

        // Details action to show one customer's details (for pop-up or similar)
        [HttpGet]
        public IActionResult Details(int id)
        {
            var customer = _repository.GetCustomerDetails(id);
            if (customer == null)
                return NotFound("Customer not found.");
            return Json(customer);
        }

        // Orders action: returns a view with the top 3 orders (by total cost) for the given customer
        public IActionResult Orders(int id)
        {
            string customerName = _repository.GetCustomerName(id);
            if (string.IsNullOrEmpty(customerName))
            {
                return NotFound("Customer not found.");
            }

            var rows = _repository.GetCustomerOrders(id);
            var grouped = rows.GroupBy(r => r.OrderId);
            var orders = new List<Order>();

            foreach (var grp in grouped)
            {
                var firstRow = grp.First();
                var order = new Order
                {
                    OrderId = firstRow.OrderId,
                    OrderDate = firstRow.OrderDate,
                    OrderTotal = firstRow.OrderTotal,
                    Items = grp.Select(r => new OrderItem
                    {
                        // Using the consolidated OrderItem model which now includes ProductName
                        ItemId = r.ItemId,
                        ProductName = r.ProductName,
                        Quantity = r.Quantity,
                        ListPrice = r.ListPrice,
                        // Optionally set ProductId and Discount if available
                        ProductId = 0,
                        Discount = 0
                    }).ToList()
                };
                orders.Add(order);
            }

            var viewModel = new OrdersViewModel
            {
                CustomerName = customerName,
                Orders = orders
            };

            return View(viewModel);
        }
    }
}
