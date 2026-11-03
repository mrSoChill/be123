using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PagedList;

namespace demoBE.Models.ViewModel
{
    public class ProductDetailVM
    {
        public ProductDetailVM() { RelatedProducts = new List<Product>(); }
        public Product Product { get; set; }
        [Range(1, 99, ErrorMessage = "Số lượng phải nằm trong khoảng từ 1 đến 99.")]
        public int Quantity { get; set; }
        public int UnitsSold { get; set; }
        public decimal EstimatedValue { get { return Product == null ? 0 : Product.ProductPrice * Quantity; } }
        public List<Product> RelatedProducts { get; set; }
        public IPagedList<Product> TopProducts { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
