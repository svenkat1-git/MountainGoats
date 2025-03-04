using System;
using System.Data;  // for CommandType
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MountainGoatsBikes.Models;

namespace MountainGoatsBikes.Repositories
{
    public class Repository
    {
        private readonly string _connectionString;

        public Repository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BikeStores")
                ?? throw new InvalidOperationException("No BikeStores connection string found in configuration.");
        }

        public IEnumerable<Brand> GetBrands()
        {
            var brands = new List<Brand>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT brand_id, brand_name FROM production.brands";
                var cmd = new SqlCommand(query, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
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
            return brands;
        }

        public IEnumerable<Category> GetCategories()
        {
            var categories = new List<Category>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT category_id, category_name FROM production.categories";
                var cmd = new SqlCommand(query, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
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
            return categories;
        }

        public int GetTotalCustomerCount()
        {
            int count = 0;
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM sales.customers";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    count = (int)cmd.ExecuteScalar();
                }
            }
            return count;
        }

        public IEnumerable<Customer> GetCustomersPaged(int offset, int pageSize)
        {
            var customers = new List<Customer>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT customer_id, first_name, last_name, phone, email, street, city, state, zip_code
                    FROM sales.customers
                    ORDER BY customer_id
                    OFFSET @offset ROWS
                    FETCH NEXT @pageSize ROWS ONLY;";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@offset", offset);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                CustomerId = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Email = reader.GetString(4),
                                Street = reader.IsDBNull(5) ? null : reader.GetString(5),
                                City = reader.IsDBNull(6) ? null : reader.GetString(6),
                                State = reader.IsDBNull(7) ? null : reader.GetString(7),
                                ZipCode = reader.IsDBNull(8) ? null : reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return customers;
        }

        public Customer? GetCustomerDetails(int id)
        {
            Customer? customer = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT customer_id, first_name, last_name, phone, email, street, city, state, zip_code
                    FROM sales.customers
                    WHERE customer_id = @id;";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customer = new Customer
                            {
                                CustomerId = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Email = reader.GetString(4),
                                Street = reader.IsDBNull(5) ? null : reader.GetString(5),
                                City = reader.IsDBNull(6) ? null : reader.GetString(6),
                                State = reader.IsDBNull(7) ? null : reader.GetString(7),
                                ZipCode = reader.IsDBNull(8) ? null : reader.GetString(8)
                            };
                        }
                    }
                }
            }
            return customer;
        }

        public string GetCustomerName(int id)
        {
            string name = "";
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT first_name, last_name FROM sales.customers WHERE customer_id = @id;";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            name = reader.GetString(0) + " " + reader.GetString(1);
                        }
                    }
                }
            }
            return name;
        }

        // Existing method that gets top 3 orders by total cost using direct query
        public IEnumerable<CustomerOrderRow> GetCustomerOrders(int id)
        {
            var rows = new List<CustomerOrderRow>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT top_orders.order_id,
                           top_orders.order_total,
                           o.order_date,
                           oi.item_id,
                           p.product_name,
                           oi.quantity,
                           oi.list_price
                    FROM
                    (
                        SELECT TOP 3 o2.order_id,
                                     SUM(oi2.quantity * oi2.list_price) AS order_total
                        FROM sales.orders o2
                        JOIN sales.order_items oi2 ON o2.order_id = oi2.order_id
                        WHERE o2.customer_id = @custId
                        GROUP BY o2.order_id
                        ORDER BY SUM(oi2.quantity * oi2.list_price) DESC
                    ) AS top_orders
                    JOIN sales.orders o ON top_orders.order_id = o.order_id
                    JOIN sales.order_items oi ON o.order_id = oi.order_id
                    JOIN production.products p ON p.product_id = oi.product_id
                    WHERE o.customer_id = @custId
                    ORDER BY top_orders.order_total DESC, top_orders.order_id, oi.item_id;";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@custId", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rows.Add(new CustomerOrderRow
                            {
                                OrderId = reader.GetInt32(0),
                                OrderTotal = reader.GetDecimal(1),
                                OrderDate = reader.GetDateTime(2),
                                ItemId = reader.GetInt32(3),
                                ProductName = reader.GetString(4),
                                Quantity = reader.GetInt32(5),
                                ListPrice = reader.GetDecimal(6)
                            });
                        }
                    }
                }
            }
            return rows;
        }

        // ******************************************************************
        // NEW METHOD that calls the stored procedure "sales.proc_cust_order_details"
        // ******************************************************************
        public IEnumerable<CustomerOrderRow> GetCustomerOrdersProc(int customerId, DateTime startDate, DateTime endDate)
        {
            var rows = new List<CustomerOrderRow>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sales.proc_cust_order_details", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    // If the stored procedure raises an error or hits the CATCH block,
                    // it returns a single column named "ErrorMessage"
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // If we detect the single "ErrorMessage" column, throw it as an exception
                            if (reader.FieldCount == 1 && reader.GetName(0).Equals("ErrorMessage", StringComparison.OrdinalIgnoreCase))
                            {
                                string errorMsg = reader.GetString(0);
                                throw new Exception(errorMsg);
                            }
                            else
                            {
                                // Otherwise, it's a normal row with order data
                                var row = new CustomerOrderRow
                                {
                                    OrderId = reader.GetInt32(0),
                                    OrderTotal = reader.GetDecimal(1),
                                    OrderDate = reader.GetDateTime(2),
                                    ItemId = reader.GetInt32(3),
                                    ProductName = reader.GetString(4),
                                    Quantity = reader.GetInt32(5),
                                    ListPrice = reader.GetDecimal(6)
                                };
                                rows.Add(row);
                            }
                        }
                    }
                }
            }

            return rows;
        }

        public IEnumerable<OrderItem> GetOrderItems()
        {
            var items = new List<OrderItem>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT order_id, item_id, product_id, quantity, list_price, discount, '' AS product_name FROM sales.order_items";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new OrderItem
                            {
                                OrderId = reader.GetInt32(0),
                                ItemId = reader.GetInt32(1),
                                ProductId = reader.GetInt32(2),
                                Quantity = reader.GetInt32(3),
                                ListPrice = reader.GetDecimal(4),
                                Discount = reader.GetDecimal(5),
                                ProductName = reader.GetString(6)
                            });
                        }
                    }
                }
            }
            return items;
        }

        public IEnumerable<Order> GetOrders()
        {
            var orders = new List<Order>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT order_id, customer_id, order_status, order_date, required_date, shipped_date, store_id, staff_id FROM sales.orders";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                OrderId = reader.GetInt32(0),
                                CustomerId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
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
            }
            return orders;
        }

        public IEnumerable<OrderSatisfaction> GetOrderSatisfactions()
        {
            var satisfactions = new List<OrderSatisfaction>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT sat_id, satisfaction_level, order_id FROM sales.order_satisfaction";
                var cmd = new SqlCommand(query, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        satisfactions.Add(new OrderSatisfaction
                        {
                            SatId = reader.GetInt32(0),
                            SatisfactionLevel = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            OrderId = reader.IsDBNull(2) ? null : reader.GetInt32(2)
                        });
                    }
                }
            }
            return satisfactions;
        }

        public IEnumerable<Product> GetProducts()
        {
            var products = new List<Product>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT product_id, product_name, brand_id, category_id, model_year, list_price FROM production.products";
                var cmd = new SqlCommand(query, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
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
            return products;
        }

        public IEnumerable<Staff> GetStaffs()
        {
            var staffs = new List<Staff>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT staff_id, first_name, last_name, email, phone, active, store_id, manager_id FROM sales.staffs";
                var cmd = new SqlCommand(query, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        staffs.Add(new Staff
                        {
                            StaffId = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Active = reader.GetByte(5),
                            StoreId = reader.GetInt32(6),
                            ManagerId = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
                        });
                    }
                }
            }
            return staffs;
        }

        public IEnumerable<Stock> GetStocks()
        {
            var stocks = new List<Stock>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT store_id, product_id, quantity FROM production.stocks";
                var cmd = new SqlCommand(query, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stocks.Add(new Stock
                        {
                            StoreId = reader.GetInt32(0),
                            ProductId = reader.GetInt32(1),
                            Quantity = reader.IsDBNull(2) ? null : reader.GetInt32(2)
                        });
                    }
                }
            }
            return stocks;
        }

        public IEnumerable<Store> GetStores()
        {
            var stores = new List<Store>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT store_id, store_name, phone, email, street, city, state, zip_code FROM sales.stores";
                var cmd = new SqlCommand(query, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stores.Add(new Store
                        {
                            StoreId = reader.GetInt32(0),
                            StoreName = reader.GetString(1),
                            Phone = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Street = reader.IsDBNull(4) ? null : reader.GetString(4),
                            City = reader.IsDBNull(5) ? null : reader.GetString(5),
                            State = reader.IsDBNull(6) ? null : reader.GetString(6),
                            ZipCode = reader.IsDBNull(7) ? null : reader.GetString(7)
                        });
                    }
                }
            }
            return stores;
        }

        // New method: SearchProductDetails
        public IEnumerable<ProductDetails> SearchProductDetails(
            int? brandId,
            int? categoryId,
            string? productName,
            string? storeZip)
        {
            var results = new List<ProductDetails>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        product_id,
                        product_name,
                        brand_id,
                        brand_name,
                        category_id,
                        category_name,
                        model_year,
                        list_price,
                        store_id,
                        store_name,
                        store_phone,
                        store_zip,
                        quantity
                    FROM production.view_product_details
                    WHERE
                        (@brandId IS NULL OR brand_id = @brandId)
                        AND (@categoryId IS NULL OR category_id = @categoryId)
                        AND (@storeZip IS NULL OR store_zip = @storeZip)
                        AND (@productName IS NULL OR product_name LIKE '%' + @productName + '%')
                    ORDER BY product_name;";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@brandId", (object?)brandId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@categoryId", (object?)categoryId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@productName", (object?)productName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@storeZip", (object?)storeZip ?? DBNull.Value);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pd = new ProductDetails
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                BrandId = reader.GetInt32(2),
                                BrandName = reader.GetString(3),
                                CategoryId = reader.GetInt32(4),
                                CategoryName = reader.GetString(5),
                                ModelYear = reader.GetInt16(6),
                                ListPrice = reader.GetDecimal(7),
                                StoreId = reader.GetInt32(8),
                                StoreName = reader.GetString(9),
                                StorePhone = reader.IsDBNull(10) ? null : reader.GetString(10),
                                StoreZip = reader.IsDBNull(11) ? null : reader.GetString(11),
                                Quantity = reader.IsDBNull(12) ? null : reader.GetInt32(12)
                            };
                            results.Add(pd);
                        }
                    }
                }
            }
            return results;
        }
    }

    // Helper model for the customer orders query
    public class CustomerOrderRow
    {
        public int OrderId { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal ListPrice { get; set; }
    }
}