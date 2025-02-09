using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
    public class CustomersController : Controller
    {
        private readonly string _connectionString;
        public CustomersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores");
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
                            Phone = reader.GetString(3),
                            Email = reader.GetString(4),
                            Street = reader.GetString(5),
                            City = reader.GetString(6),
                            State = reader.GetString(7),
                            ZipCode = reader.GetString(8)
                        });
                    }
                }
            }
            return View(customers);
        }
    }
}
