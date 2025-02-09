using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using MountainGoatsBikes.Models;
using Microsoft.Extensions.Configuration;

namespace MountainGoatsBikes.Controllers
{
    public class StaffsController : Controller
    {
        private readonly string _connectionString;
        public StaffsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores");
        }

        public IActionResult Index()
        {
            List<Staff> staffs = new List<Staff>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT staff_id, first_name, last_name, email, phone, active, store_id, manager_id FROM sales.staffs";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        staffs.Add(new Staff
                        {
                            StaffId = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Phone = reader.GetString(4),
                            Active = reader.GetByte(5),
                            StoreId = reader.GetInt32(6),
                            ManagerId = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
                        });
                    }
                }
            }
            return View(staffs);
        }
    }
}
