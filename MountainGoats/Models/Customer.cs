using System;
using System.Collections.Generic;

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

    // Used as the model for the customer list view with paging info
    public class CustomerList
    {
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }

    // View model for displaying a customer's orders
    public class OrdersViewModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
