using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
    public class BrandsController : Controller
    {
        private readonly string _connectionString;
        public BrandsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores")!;
        }

        public IActionResult Index()
        {
            List<Brand> brands = new List<Brand>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT brand_id, brand_name FROM production.brands";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        brands.Add(new Brand
                        {
                            BrandId = reader.GetInt32(0),
                            BrandName = reader.GetString(1)
                        });
                    }
                }
            }
            return View(brands);
        }
    }
}
