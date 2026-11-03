using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using demoBE.Models;
using demoBE.Models.ViewModel;
using PagedList;

namespace demoBE.Controllers
{
    public class CartController : Controller
    {
        private const int MaximumQuantity = 99;
        private const int SimilarProductPageSize = 6;
        private readonly MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index()
        {
            return View(GetCartService().GetCart());
        }

        public ActionResult Index2(int? page)
        {
            var cart = GetCartService().GetCart();
            var productIds = cart.Items.Select(item => item.ProductID).ToList();
            var categoryIds = db.Product.Where(product => productIds.Contains(product.ProductID))
                .Select(product => product.CategoryID)
                .Distinct()
                .ToList();
            var pageNumber = !page.HasValue || page.Value < 1 ? 1 : page.Value;

            var suggestions = db.Product.Include(product => product.Category)
                .Where(product => categoryIds.Contains(product.CategoryID) && !productIds.Contains(product.ProductID))
                .OrderBy(product => product.ProductID);
            var pageCount = (int)Math.Ceiling(suggestions.Count() / (decimal)SimilarProductPageSize);
            if (pageCount > 0 && pageNumber > pageCount)
            {
                pageNumber = pageCount;
            }

            cart.SimilarProducts = suggestions.ToPagedList(pageNumber, SimilarProductPageSize);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int productID, int quantity, string returnUrl)
        {
            if (quantity < 1 || quantity > MaximumQuantity)
            {
                TempData["CartError"] = "Số lượng phải nằm trong khoảng từ 1 đến 99.";
                return RedirectToLocalOrHome(returnUrl);
            }

            var product = db.Product.Include(item => item.Category)
                .SingleOrDefault(item => item.ProductID == productID);
            if (product == null)
            {
                TempData["CartError"] = "Sản phẩm không còn tồn tại.";
                return RedirectToLocalOrHome(returnUrl);
            }

            GetCartService().AddProduct(product, quantity, MaximumQuantity);
            TempData["CartSuccess"] = "Đã thêm sản phẩm vào giỏ hàng.";
            return RedirectToLocalOrHome(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int productID, int quantity, bool advanced = false)
        {
            if (!GetCartService().GetCart().UpdateQuantity(productID, quantity, MaximumQuantity))
            {
                TempData["CartError"] = "Không thể cập nhật số lượng. Giá trị hợp lệ từ 1 đến 99.";
            }
            else
            {
                TempData["CartSuccess"] = "Đã cập nhật số lượng.";
            }

            return RedirectToCart(advanced);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSelection(int productID, bool isSelected, bool advanced = false)
        {
            if (!GetCartService().GetCart().UpdateSelection(productID, isSelected))
            {
                TempData["CartError"] = "Không tìm thấy mặt hàng trong giỏ.";
            }

            return RedirectToCart(advanced);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromCart(int productID, bool advanced = false)
        {
            if (!GetCartService().GetCart().RemoveItem(productID))
            {
                TempData["CartError"] = "Không tìm thấy mặt hàng trong giỏ.";
            }
            else
            {
                TempData["CartSuccess"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            }

            return RedirectToCart(advanced);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClearCart(bool advanced = false)
        {
            GetCartService().Clear();
            TempData["CartSuccess"] = "Đã xóa toàn bộ giỏ hàng.";
            return RedirectToCart(advanced);
        }

        public ActionResult Checkout()
        {
            return View(GetCartService().GetCart());
        }

        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        private ActionResult RedirectToCart(bool advanced)
        {
            return advanced ? (ActionResult)RedirectToAction("Index2") : RedirectToAction("Index");
        }

        private ActionResult RedirectToLocalOrHome(string returnUrl)
        {
            return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? (ActionResult)Redirect(returnUrl)
                : RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
