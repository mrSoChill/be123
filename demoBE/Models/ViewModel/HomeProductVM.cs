using System.Collections.Generic;
using PagedList;

namespace demoBE.Models.ViewModel
{
    public class HomeProductVM
    {
        public HomeProductVM() { FeaturedProducts = new List<Product>(); }
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalMatchingProducts { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public IPagedList<Product> NewProducts { get; set; }
    }
}
