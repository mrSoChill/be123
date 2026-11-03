namespace demoBE.Models.ViewModel
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsSelected { get; set; }
        public decimal TotalPrice { get { return UnitPrice * Quantity; } }
    }
}
