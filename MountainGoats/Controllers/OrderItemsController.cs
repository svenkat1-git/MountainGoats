using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly string _connectionString;
        public OrderItemsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores");
        }

        public IActionResult Index()
        {
            List<OrderItem> orderItems = new List<OrderItem>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT order_id, item_id, product_id, quantity, list_price, discount FROM sales.order_items";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orderItems.Add(new OrderItem
                        {
                            OrderId = reader.GetInt32(0),
                            ItemId = reader.GetInt32(1),
                            ProductId = reader.GetInt32(2),
                            Quantity = reader.GetInt32(3),
                            ListPrice = reader.GetDecimal(4),
                            Discount = reader.GetDecimal(5)
                        });
                    }
                }
            }
            return View(orderItems);
        }
    }
}
