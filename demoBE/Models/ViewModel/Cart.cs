using System.Collections.Generic;
using System.Linq;
using PagedList;

namespace demoBE.Models.ViewModel
{
    public class Cart
    {
        public Cart()
        {
            Items = new List<CartItem>();
        }

        public List<CartItem> Items { get; set; }
        public IPagedList<Product> SimilarProducts { get; set; }
        public IEnumerable<IGrouping<string, CartItem>> GroupedItems
        {
            get { return Items.GroupBy(item => string.IsNullOrWhiteSpace(item.Category) ? "Chưa phân loại" : item.Category); }
        }
        public int TotalQuantity { get { return Items.Sum(item => item.Quantity); } }
        public decimal TotalValue { get { return Items.Sum(item => item.TotalPrice); } }
        public decimal SelectedTotalValue { get { return Items.Where(item => item.IsSelected).Sum(item => item.TotalPrice); } }

        public void AddItem(CartItem item, int maximumQuantity)
        {
            var existing = Items.SingleOrDefault(current => current.ProductID == item.ProductID);
            if (existing == null)
            {
                item.Quantity = ClampQuantity(item.Quantity, maximumQuantity);
                item.IsSelected = true;
                Items.Add(item);
                return;
            }

            existing.Quantity = ClampQuantity(existing.Quantity + item.Quantity, maximumQuantity);
            existing.ProductName = item.ProductName;
            existing.ProductImage = item.ProductImage;
            existing.Category = item.Category;
            existing.UnitPrice = item.UnitPrice;
        }

        public bool UpdateQuantity(int productId, int quantity, int maximumQuantity)
        {
            var item = Items.SingleOrDefault(current => current.ProductID == productId);
            if (item == null || quantity < 1 || quantity > maximumQuantity)
            {
                return false;
            }

            item.Quantity = quantity;
            return true;
        }

        public bool UpdateSelection(int productId, bool isSelected)
        {
            var item = Items.SingleOrDefault(current => current.ProductID == productId);
            if (item == null)
            {
                return false;
            }

            item.IsSelected = isSelected;
            return true;
        }

        public bool RemoveItem(int productId)
        {
            var item = Items.SingleOrDefault(current => current.ProductID == productId);
            if (item == null)
            {
                return false;
            }

            Items.Remove(item);
            return true;
        }

        public void Clear() { Items.Clear(); }

        private static int ClampQuantity(int quantity, int maximumQuantity)
        {
            return quantity < 1 ? 1 : (quantity > maximumQuantity ? maximumQuantity : quantity);
        }
    }
}
