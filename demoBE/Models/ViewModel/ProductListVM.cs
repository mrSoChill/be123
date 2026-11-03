using PagedList;

namespace demoBE.Models.ViewModel
{
    public class ProductListVM
    {
        public Category Category { get; set; }
        public string SearchTerm { get; set; }
        public string SortOrder { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IPagedList<Product> Products { get; set; }
    }
}
