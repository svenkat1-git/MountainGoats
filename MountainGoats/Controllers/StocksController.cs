using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
    public class StocksController : Controller
    {
        private readonly string _connectionString;
        public StocksController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores");
        }

        public IActionResult Index()
        {
            List<Stock> stocks = new List<Stock>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT store_id, product_id, quantity FROM production.stocks";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stocks.Add(new Stock
                        {
                            StoreId = reader.GetInt32(0),
                            ProductId = reader.GetInt32(1),
                            Quantity = reader.GetInt32(2)
                        });
                    }
                }
            }
            return View(stocks);
        }
    }
}
