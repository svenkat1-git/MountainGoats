using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
    public class StoresController : Controller
    {
        private readonly string _connectionString;
        public StoresController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores");
        }

        public IActionResult Index()
        {
            List<Store> stores = new List<Store>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT store_id, store_name, phone, email, street, city, state, zip_code FROM sales.stores";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stores.Add(new Store
                        {
                            StoreId = reader.GetInt32(0),
                            StoreName = reader.GetString(1),
                            Phone = reader.GetString(2),
                            Email = reader.GetString(3),
                            Street = reader.GetString(4),
                            City = reader.GetString(5),
                            State = reader.GetString(6),
                            ZipCode = reader.GetString(7)
                        });
                    }
                }
            }
            return View(stores);
        }
    }
}
