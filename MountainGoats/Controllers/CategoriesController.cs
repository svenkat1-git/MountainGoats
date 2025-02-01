using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MountainGoats.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MountainGoats.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly string _connectionString;

        public CategoriesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStoresConnection");
        }

        public IActionResult Index()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT category_id, category_name FROM production.categories";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                CategoryId = reader.GetInt32(0),
                                CategoryName = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return View(categories);
        }
    }
}
