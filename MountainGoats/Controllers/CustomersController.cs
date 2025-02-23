using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace MountainGoatsBikes.Controllers
{
    public class CustomersController : Controller
    {
        private readonly string _connectionString;

        public CustomersController(IConfiguration configuration)
        {
<<<<<<< HEAD
            // Keep existing code, do not rename or change unnecessarily
            _connectionString = configuration.GetConnectionString("BikeStores")!;
        }

        // Existing Index method for listing/paging customers remains unchanged

=======
            _connectionString = configuration.GetConnectionString("BikeStores")!;
        }

        public IActionResult Index(int page = 1)
        {
            const int pageSize = 10; // number of customers per page

            // 1. Get total number of customer records
            int totalRecords = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string countQuery = "SELECT COUNT(*) FROM sales.customers";
                SqlCommand countCmd = new SqlCommand(countQuery, conn);
                conn.Open();
                totalRecords = (int)countCmd.ExecuteScalar();
            }

            // 2. Calculate offset for the current page
            int offset = (page - 1) * pageSize;
            List<Customer> customers = new List<Customer>();

            // 3. Retrieve the customer records for the current page using OFFSET/FETCH
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT customer_id, first_name, last_name, phone, email, street, city, state, zip_code
                    FROM sales.customers
                    ORDER BY customer_id
                    OFFSET @offset ROWS
                    FETCH NEXT @pageSize ROWS ONLY;
                ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@offset", offset);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            CustomerId = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Email = reader.GetString(4),
                            Street = reader.IsDBNull(5) ? null : reader.GetString(5),
                            City = reader.IsDBNull(6) ? null : reader.GetString(6),
                            State = reader.IsDBNull(7) ? null : reader.GetString(7),
                            ZipCode = reader.IsDBNull(8) ? null : reader.GetString(8)
                        });
                    }
                }
            }

            // 4. Build the CustomerList model with paging info
            var model = new CustomerList
            {
                Customers = customers,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return View(model);
        }

        // Existing method for returning a single customer's details as JSON
>>>>>>> 7cd028b (Added a new Orders(int id) action to retrieve the top 3 orders by total cost for the given customer, including each order’s line items. This uses one query with a subselect for the top 3, joined to order_items and products.)
        [HttpGet]
        public IActionResult Details(int id)
        {
            // Existing code for returning JSON with one customer
            // ...
        }

        // New Orders action: top 3 orders by total cost
        public IActionResult Orders(int id)
        {
            // 1) Retrieve the customer's name
            string customerName;
            using (var conn = new SqlConnection(_connectionString))
            {
                string custQuery = @"
                    SELECT first_name, last_name
                    FROM sales.customers
                    WHERE customer_id = @id
                ";
                var cmd = new SqlCommand(custQuery, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        customerName = reader.GetString(0) + " " + reader.GetString(1);
                    }
                    else
                    {
                        return NotFound("Customer not found.");
                    }
                }
            }

            // 2) Single query: find top 3 orders (by total cost) + line items
            //    We'll group them in code.
            var rows = new List<(
                int OrderId,
                decimal OrderTotal,
                DateTime OrderDate,
                int ItemId,
                string ProductName,
                int Quantity,
                decimal ListPrice
            )>();

            using (var conn = new SqlConnection(_connectionString))
            {
<<<<<<< HEAD
                // We do a subselect that calculates the top 3 order_ids by total cost, 
                // then join them back to get line items.
                string query = @"
                    SELECT top_orders.order_id,
                           top_orders.order_total,
                           o.order_date,
                           oi.item_id,
                           p.product_name,
                           oi.quantity,
                           oi.list_price
                    FROM
                    (
                        SELECT TOP 3 o2.order_id,
                                     SUM(oi2.quantity * oi2.list_price) AS order_total
                        FROM sales.orders o2
                        JOIN sales.order_items oi2 ON o2.order_id = oi2.order_id
                        WHERE o2.customer_id = @custId
                        GROUP BY o2.order_id
                        ORDER BY SUM(oi2.quantity * oi2.list_price) DESC
                    ) AS top_orders
                    JOIN sales.orders o ON top_orders.order_id = o.order_id
                    JOIN sales.order_items oi ON o.order_id = oi.order_id
                    JOIN production.products p ON p.product_id = oi.product_id
                    WHERE o.customer_id = @custId
                    ORDER BY top_orders.order_total DESC, top_orders.order_id, oi.item_id;
                ";

                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@custId", id);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add((
                            OrderId: reader.GetInt32(0),
                            OrderTotal: reader.GetDecimal(1),
                            OrderDate: reader.GetDateTime(2),
                            ItemId: reader.GetInt32(3),
                            ProductName: reader.GetString(4),
                            Quantity: reader.GetInt32(5),
                            ListPrice: reader.GetDecimal(6)
                        ));
                    }
                }
            }

            // 3) Group the rows by OrderId to build a list of Order objects
            var orders = new List<Order>();
            var grouped = rows.GroupBy(r => r.OrderId);

            foreach (var grp in grouped)
            {
                var firstRow = grp.First(); // all have same order_id
=======
                return NotFound();
            }

            return Json(customer);
        }

        // New action to show top 3 orders by total cost, in descending order
        public IActionResult Orders(int id)
        {
            // 1) Retrieve the customer's name
            string customerName;
            using (var conn = new SqlConnection(_connectionString))
            {
                string custQuery = @"
                    SELECT first_name, last_name
                    FROM sales.customers
                    WHERE customer_id = @id;
                ";
                var cmd = new SqlCommand(custQuery, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        customerName = reader.GetString(0) + " " + reader.GetString(1);
                    }
                    else
                    {
                        return NotFound("Customer not found.");
                    }
                }
            }

            // 2) Single query: find top 3 orders (by total cost) + line items
            var rows = new List<(
                int OrderId,
                decimal OrderTotal,
                DateTime OrderDate,
                int ItemId,
                string ProductName,
                int Quantity,
                decimal ListPrice
            )>();

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT top_orders.order_id,
                           top_orders.order_total,
                           o.order_date,
                           oi.item_id,
                           p.product_name,
                           oi.quantity,
                           oi.list_price
                    FROM
                    (
                        SELECT TOP 3 o2.order_id,
                                     SUM(oi2.quantity * oi2.list_price) AS order_total
                        FROM sales.orders o2
                        JOIN sales.order_items oi2 ON o2.order_id = oi2.order_id
                        WHERE o2.customer_id = @custId
                        GROUP BY o2.order_id
                        ORDER BY SUM(oi2.quantity * oi2.list_price) DESC
                    ) AS top_orders
                    JOIN sales.orders o ON top_orders.order_id = o.order_id
                    JOIN sales.order_items oi ON o.order_id = oi.order_id
                    JOIN production.products p ON p.product_id = oi.product_id
                    WHERE o.customer_id = @custId
                    ORDER BY top_orders.order_total DESC, top_orders.order_id, oi.item_id;
                ";

                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@custId", id);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add((
                            OrderId: reader.GetInt32(0),
                            OrderTotal: reader.GetDecimal(1),
                            OrderDate: reader.GetDateTime(2),
                            ItemId: reader.GetInt32(3),
                            ProductName: reader.GetString(4),
                            Quantity: reader.GetInt32(5),
                            ListPrice: reader.GetDecimal(6)
                        ));
                    }
                }
            }

            // 3) Group these rows by OrderId to build a list of Order objects
            var orders = new List<Order>();
            var grouped = rows.GroupBy(r => r.OrderId);

            foreach (var grp in grouped)
            {
                var firstRow = grp.First();
>>>>>>> 7cd028b (Added a new Orders(int id) action to retrieve the top 3 orders by total cost for the given customer, including each order’s line items. This uses one query with a subselect for the top 3, joined to order_items and products.)
                var order = new Order
                {
                    OrderId = firstRow.OrderId,
                    OrderDate = firstRow.OrderDate,
                    OrderTotal = firstRow.OrderTotal
                };

                // Add each line item
                foreach (var row in grp)
                {
                    order.Items.Add(new OrderItem
                    {
                        ItemId = row.ItemId,
                        ProductName = row.ProductName,
                        Quantity = row.Quantity,
                        ListPrice = row.ListPrice
                    });
                }

                orders.Add(order);
            }

<<<<<<< HEAD
            // 4) Build the view model with the customer's name and these top 3 orders
=======
            // 4) Create a view model with the customer's name and these orders
>>>>>>> 7cd028b (Added a new Orders(int id) action to retrieve the top 3 orders by total cost for the given customer, including each order’s line items. This uses one query with a subselect for the top 3, joined to order_items and products.)
            var viewModel = new OrdersViewModel
            {
                CustomerName = customerName,
                Orders = orders
            };

<<<<<<< HEAD
            // 5) Return a new Razor page named "Orders" (Views/Customers/Orders.cshtml)
            return View(viewModel);
=======
            return View(viewModel); // Return the new Orders.cshtml
>>>>>>> 7cd028b (Added a new Orders(int id) action to retrieve the top 3 orders by total cost for the given customer, including each order’s line items. This uses one query with a subselect for the top 3, joined to order_items and products.)
        }
    }
}