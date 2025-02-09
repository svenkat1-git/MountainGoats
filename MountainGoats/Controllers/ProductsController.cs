using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
	public class ProductsController : Controller
	{
		private readonly string _connectionString;
		public ProductsController(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("BikeStores");
		}

		public IActionResult Index()
		{
			List<Product> products = new List<Product>();
			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				string query = "SELECT product_id, product_name, brand_id, category_id, model_year, list_price FROM production.products";
				SqlCommand cmd = new SqlCommand(query, conn);
				conn.Open();
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						products.Add(new Product
						{
							ProductId = reader.GetInt32(0),
							ProductName = reader.GetString(1),
							BrandId = reader.GetInt32(2),
							CategoryId = reader.GetInt32(3),
							ModelYear = reader.GetInt16(4),
							ListPrice = reader.GetDecimal(5)
						});
					}
				}
			}
			return View(products);
		}
	}
}
