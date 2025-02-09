using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
    public class OrderSatisfactionController : Controller
    {
        private readonly string _connectionString;
        public OrderSatisfactionController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores");
        }

        public IActionResult Index()
        {
            List<OrderSatisfaction> satisfactions = new List<OrderSatisfaction>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT sat_id, satisfaction_level, order_id FROM sales.order_satisfaction";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        satisfactions.Add(new OrderSatisfaction
                        {
                            SatId = reader.GetInt32(0),
                            SatisfactionLevel = reader.GetInt32(1),
                            OrderId = reader.GetInt32(2)
                        });
                    }
                }
            }
            return View(satisfactions);
        }
    }
}
