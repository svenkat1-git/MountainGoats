using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MountainGoatsBikes.Models
{
    public class ProductSearchViewModel
    {
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public string? ProductName { get; set; }
        public string? StoreZip { get; set; }

        // Results from the search query
        public IEnumerable<ProductDetails> Results { get; set; } = new List<ProductDetails>();

        // Dropdown lists populated from the database
        public IEnumerable<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> StoreZips { get; set; } = new List<SelectListItem>();
    }
}
