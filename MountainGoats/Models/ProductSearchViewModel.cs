using System.Collections.Generic;

namespace MountainGoatsBikes.Models
{
    public class ProductSearchViewModel
    {
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public string? ProductName { get; set; }
        public string? StoreZip { get; set; }

        public IEnumerable<ProductDetails> Results { get; set; } = new List<ProductDetails>();
    }
}
