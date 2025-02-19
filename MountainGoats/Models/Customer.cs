namespace MountainGoatsBikes.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
    }

    // Simple paging container used as the model for the view
    public class CustomerList
    {
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }

    // Minimal classes for the new Orders page:
    public class OrderItem
    {
        public int ItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal ListPrice { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderTotal { get; set; }  // The total cost of this order
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrdersViewModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
