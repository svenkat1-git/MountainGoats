using System;
using System.Collections.Generic;

namespace MountainGoatsBikes.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public byte OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int StoreId { get; set; }
        public int StaffId { get; set; }

        // Additional property used in the Orders view
        public decimal OrderTotal { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
