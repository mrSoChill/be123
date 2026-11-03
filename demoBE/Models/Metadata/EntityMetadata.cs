using System;
using System.ComponentModel.DataAnnotations;

namespace demoBE.Models
{
    [MetadataType(typeof(CategoryMetadata))]
    public partial class Category { }
    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer { }
    [MetadataType(typeof(OrderMetadata))]
    public partial class Order { }
    [MetadataType(typeof(OrderDetailMetadata))]
    public partial class OrderDetail { }
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product { }
    [MetadataType(typeof(UserMetadata))]
    public partial class User { }

    internal sealed class CategoryMetadata
    {
        [Display(Name = "Mã danh mục")] public int CategoryID { get; set; }
        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        [Display(Name = "Tên danh mục")] public string CategoryName { get; set; }
    }

    internal sealed class CustomerMetadata
    {
        [Display(Name = "Mã khách hàng")] public int CustomerID { get; set; }
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc.")]
        [Display(Name = "Tên khách hàng")] public string CustomerName { get; set; }
        [DataType(DataType.PhoneNumber)] [Display(Name = "Số điện thoại")] public string CustomerPhone { get; set; }
        [DataType(DataType.EmailAddress)] [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [Display(Name = "Email")] public string CustomerEmail { get; set; }
        [DataType(DataType.MultilineText)] [Display(Name = "Địa chỉ")] public string CustomerAddress { get; set; }
        [Display(Name = "Tài khoản")] public string Username { get; set; }
    }

    internal sealed class OrderMetadata
    {
        [Display(Name = "Mã đơn hàng")] public int OrderID { get; set; }
        [Display(Name = "Khách hàng")] public int CustomerID { get; set; }
        [DataType(DataType.DateTime)] [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày đặt")] public DateTime OrderDate { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền không được âm.")]
        [DataType(DataType.Currency)] [DisplayFormat(DataFormatString = "{0:N0} ₫")]
        [Display(Name = "Tổng tiền")] public decimal TotalAmount { get; set; }
        [Display(Name = "Trạng thái thanh toán")] public string PaymentStatus { get; set; }
        [DataType(DataType.MultilineText)] [Display(Name = "Địa chỉ giao hàng")] public string AddressDelivery { get; set; }
    }

    internal sealed class OrderDetailMetadata
    {
        [Display(Name = "Mã chi tiết")] public int ID { get; set; }
        [Display(Name = "Sản phẩm")] public int ProductID { get; set; }
        [Display(Name = "Đơn hàng")] public int OrderID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        [Display(Name = "Số lượng")] public int Quantity { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không được âm.")]
        [DataType(DataType.Currency)] [DisplayFormat(DataFormatString = "{0:N0} ₫")]
        [Display(Name = "Đơn giá")] public decimal UnitPrice { get; set; }
    }

    internal sealed class ProductMetadata
    {
        [Display(Name = "Mã sản phẩm")] public int ProductID { get; set; }
        [Display(Name = "Danh mục")] public int CategoryID { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [Display(Name = "Tên sản phẩm")] public string ProductName { get; set; }
        [Required(ErrorMessage = "Mô tả sản phẩm là bắt buộc.")]
        [DataType(DataType.MultilineText)] [Display(Name = "Mô tả sản phẩm")]
        public string ProductDecription { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        [DataType(DataType.Currency)] [DisplayFormat(DataFormatString = "{0:N0} ₫")]
        [Display(Name = "Giá sản phẩm")] public decimal ProductPrice { get; set; }
        [Display(Name = "Ảnh sản phẩm")] public string ProductImage { get; set; }
    }

    internal sealed class UserMetadata
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [Display(Name = "Tên đăng nhập")] public string Username { get; set; }
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)] [Display(Name = "Mật khẩu")] public string Password { get; set; }
        [Display(Name = "Vai trò")] public string UserRole { get; set; }
    }
}
