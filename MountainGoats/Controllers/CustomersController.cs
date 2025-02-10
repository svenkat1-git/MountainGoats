using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
    public class CustomersController : Controller
    {
        private readonly string _connectionString;
        public CustomersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores")!;
        }

        public IActionResult Index()
        {
            List<Customer> customers = new List<Customer>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT customer_id, first_name, last_name, phone, email, street, city, state, zip_code FROM sales.customers";
                SqlCommand cmd = new SqlCommand(query, conn);
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
            return View(customers);
        }
    }
}
