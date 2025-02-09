using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace MountainGoatsBikes.Controllers
{
    public class OrdersController : Controller
    {
        private readonly string _connectionString;
        public OrdersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores");
        }

        public IActionResult Index()
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT order_id, customer_id, order_status, order_date, required_date, shipped_date, store_id, staff_id FROM sales.orders";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            OrderId = reader.GetInt32(0),
                            CustomerId = reader.GetInt32(1),
                            OrderStatus = reader.GetByte(2),
                            OrderDate = reader.GetDateTime(3),
                            RequiredDate = reader.GetDateTime(4),
                            ShippedDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                            StoreId = reader.GetInt32(6),
                            StaffId = reader.GetInt32(7)
                        });
                    }
                }
            }
            return View(orders);
        }
    }
}
