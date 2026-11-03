using System.Web;

namespace demoBE.Models.ViewModel
{
    public class CartService
    {
        private const string CartSessionKey = "Cart";
        private readonly HttpSessionStateBase session;

        public CartService(HttpSessionStateBase session)
        {
            this.session = session;
        }

        public Cart GetCart()
        {
            var cart = session[CartSessionKey] as Cart;
            if (cart == null)
            {
                cart = new Cart();
                session[CartSessionKey] = cart;
            }

            return cart;
        }

        public void AddProduct(Product product, int quantity, int maximumQuantity)
        {
            GetCart().AddItem(new CartItem
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                ProductImage = product.ProductImage,
                Category = product.Category == null ? "Chưa phân loại" : product.Category.CategoryName,
                Quantity = quantity,
                UnitPrice = product.ProductPrice,
                IsSelected = true
            }, maximumQuantity);
        }

        public void Clear()
        {
            GetCart().Clear();
        }
    }
}
