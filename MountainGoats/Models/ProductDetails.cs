namespace MountainGoatsBikes.Models
{
    public class ProductDetails
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        
        public short ModelYear { get; set; }
        public decimal ListPrice { get; set; }
        
        public int StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string? StorePhone { get; set; }
        public string? StoreZip { get; set; }
        
        public int? Quantity { get; set; }
    }
}
