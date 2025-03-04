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

        // Orders action: calls the new stored-procedure-based method 
        // to retrieve top 3 orders in the 2016-2017 date range.
        public IActionResult Orders(int id)
        {
            string customerName = _repository.GetCustomerName(id);
            if (string.IsNullOrEmpty(customerName))
            {
                return NotFound("Customer not found.");
            }

            // We'll collect the final Order objects here
            var orders = new List<Order>();

            try
            {
                // Call the new procedure-based method, specifying the date range for 2016-2017
                var rows = _repository.GetCustomerOrdersProc(
                    id,
                    new DateTime(2016, 1, 1),
                    new DateTime(2017, 12, 31)
                );

                // If the stored procedure returned zero rows, we simply have an empty list
                if (!rows.Any())
                {
                    // No orders found
                }
                else
                {
                    // Group rows by order_id and build your Order objects
                    var grouped = rows.GroupBy(r => r.OrderId);
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
                // If the proc returned an error or we had a problem parsing data,
                // display it
                ViewBag.ErrorMessage = ex.Message;
            }

            // Prepare the OrdersViewModel as usual
            var viewModel = new OrdersViewModel
            {
                CustomerName = customerName,
                Orders = orders
            };

            return View(viewModel);
        }
    }
}
