using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace MountainGoatsBikes.Controllers
{
    public class CustomersController : Controller
    {
        private readonly string _connectionString;

        public CustomersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores")!;
        }

        // Accepts a page parameter from the query string (default is 1)
        public IActionResult Index(int page = 1)
        {
            const int pageSize = 10; // Number of customers per page

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

        // Placeholder action for "Details" button
        public IActionResult Details(int id)
        {
            // Retrieve a single customer's details as needed
            return View();
        }

        // Placeholder action for "Orders" button
        public IActionResult Orders(int id)
        {
            // Retrieve and display orders for the given customer
            return View();
        }
    }
}
