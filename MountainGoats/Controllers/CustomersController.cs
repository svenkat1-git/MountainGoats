using Microsoft.AspNetCore.Mvc;
using MountainGoatsBikes.Repositories;
using MountainGoatsBikes.Models;
using System.Linq;
using System.Collections.Generic;
using System;

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

        // Orders action: retrieves the three most recent orders (by descending date) for a given customer.
        public IActionResult Orders(int id)
        {
            string customerName = _repository.GetCustomerName(id);
            if (string.IsNullOrEmpty(customerName))
            {
                return NotFound("Customer not found.");
            }

            var orders = new List<Order>();

            try
            {
                // Retrieve all orders for this customer over a wide date range.
                var rows = _repository.GetCustomerOrdersProc(id, new DateTime(1900, 1, 1), DateTime.Today);

                if (rows.Any())
                {
                    // Group rows by OrderId.
                    var grouped = rows.GroupBy(r => r.OrderId);

                    // Order the groups by descending OrderDate (from the first row of each group)
                    // and take the top 3.
                    var top3Groups = grouped.OrderByDescending(g => g.First().OrderDate).Take(3);

                    foreach (var grp in top3Groups)
                    {
                        var firstRow = grp.First();
                        var order = new Order
                        {
                            OrderId = firstRow.OrderId,
                            OrderDate = firstRow.OrderDate,
                            OrderTotal = firstRow.OrderTotal,  // from your stored-procedure result
                            Items = grp.Select(r => new OrderItem
                            {
                                ItemId = r.ItemId,
                                ProductName = r.ProductName,
                                Quantity = r.Quantity,
                                ListPrice = r.ListPrice,
                                ProductId = 0,
                                Discount = 0
                            }).ToList()
                        };
                        orders.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                var errorModel = new MountainGoats.Models.ErrorViewModel { RequestId = HttpContext.TraceIdentifier };
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", errorModel);
            }

            var viewModel = new OrdersViewModel
            {
                CustomerName = customerName,
                Orders = orders
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult NewOrder(int id)
        {
            try
            {
                // Create a new order for the customer with the given id using two fixed items.
                // (The repository method NewOrder(int customerId) is updated to hard-code the two product IDs,
                // look up the store "Rowlett Bikes" and the staff "Layla Terrell".)
                _repository.NewOrder(id);
                // Redirect to the Orders action for this customer.
                return RedirectToAction("Orders", new { id = id });
            }
            catch (Exception ex)
            {
                var errorModel = new MountainGoats.Models.ErrorViewModel { RequestId = HttpContext.TraceIdentifier };
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", errorModel);
            }
        }
    }
}
