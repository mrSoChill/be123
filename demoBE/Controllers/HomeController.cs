using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using demoBE.Models;
using demoBE.Models.ViewModel;
using PagedList;

namespace demoBE.Controllers
{
    public class HomeController : Controller
    {
        private const int NewProductPageSize = 6;
        private const int ProductListPageSize = 9;
        private const int TopProductPageSize = 3;
        private const int MaximumCartQuantity = 99;
        private readonly MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(string searchTerm, int? page)
        {
            var normalizedSearch = NormalizeSearchTerm(searchTerm);
            var pageNumber = NormalizePage(page);
            var products = ApplySearch(db.Product.Include(product => product.Category), normalizedSearch).ToList();
            var sales = GetSalesByProduct();
            var leastPurchased = OrderBySales(products, sales, false).Take(20).ToList();

            var model = new HomeProductVM
            {
                SearchTerm = normalizedSearch,
                PageNumber = pageNumber,
                PageSize = NewProductPageSize,
                TotalMatchingProducts = products.Count,
                FeaturedProducts = OrderBySales(products, sales, true).Take(10).ToList(),
                NewProducts = leastPurchased.ToPagedList(pageNumber, NewProductPageSize)
            };

            return View(model);
        }

        public ActionResult ProductList(int? categoryId, string searchTerm, string sortOrder, int? page)
        {
            if (!categoryId.HasValue)
            {
                return new HttpStatusCodeResult(400, "Thiếu mã danh mục.");
            }

            var category = db.Category.Find(categoryId.Value);
            if (category == null)
            {
                return HttpNotFound();
            }

            var normalizedSearch = NormalizeSearchTerm(searchTerm);
            var normalizedSort = NormalizeSortOrder(sortOrder);
            var query = ApplySearch(db.Product.Include(product => product.Category)
                .Where(product => product.CategoryID == categoryId.Value), normalizedSearch);
            query = ApplyProductSort(query, normalizedSort);

            var pageNumber = NormalizePage(page);
            var totalPages = (int)Math.Ceiling(query.Count() / (decimal)ProductListPageSize);
            if (totalPages > 0 && pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            return View(new ProductListVM
            {
                Category = category,
                SearchTerm = normalizedSearch,
                SortOrder = normalizedSort,
                PageNumber = pageNumber,
                PageSize = ProductListPageSize,
                Products = query.ToPagedList(pageNumber, ProductListPageSize)
            });
        }

        public ActionResult ProductDetail(int? id, int? quantity, int? page)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(400, "Thiếu mã sản phẩm.");
            }

            var product = db.Product.Include(item => item.Category)
                .SingleOrDefault(item => item.ProductID == id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(BuildProductDetailModel(product, NormalizeQuantity(quantity), NormalizePage(page)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductDetail(int productID, int quantity)
        {
            if (quantity < 1 || quantity > MaximumCartQuantity)
            {
                TempData["CartError"] = "Số lượng phải nằm trong khoảng từ 1 đến 99.";
                return RedirectToAction("ProductDetail", new { id = productID });
            }

            var product = db.Product.Include(item => item.Category)
                .SingleOrDefault(item => item.ProductID == productID);
            if (product == null)
            {
                return HttpNotFound();
            }

            new CartService(Session).AddProduct(product, quantity, MaximumCartQuantity);
            TempData["CartSuccess"] = "Đã thêm sản phẩm vào giỏ hàng.";
            return RedirectToAction("Index", "Cart");
        }

        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            return RedirectToAction("ProductDetail", new { id, quantity, page });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Thông tin cửa hàng sẽ được cập nhật sau.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Thông tin liên hệ sẽ được cập nhật sau.";
            return View();
        }

        private ProductDetailVM BuildProductDetailModel(Product product, int quantity, int pageNumber)
        {
            var sales = GetSalesByProduct();
            var sameCategory = db.Product.Include(item => item.Category)
                .Where(item => item.CategoryID == product.CategoryID && item.ProductID != product.ProductID)
                .ToList();
            var topProducts = OrderBySales(sameCategory, sales, true).Take(8).ToList();
            var totalPages = (int)Math.Ceiling(topProducts.Count / (decimal)TopProductPageSize);
            if (totalPages > 0 && pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            return new ProductDetailVM
            {
                Product = product,
                Quantity = quantity,
                UnitsSold = GetUnitsSold(product.ProductID, sales),
                RelatedProducts = sameCategory.OrderBy(item => item.ProductID).Take(8).ToList(),
                TopProducts = topProducts.ToPagedList(pageNumber, TopProductPageSize),
                PageNumber = pageNumber,
                PageSize = TopProductPageSize
            };
        }

        private Dictionary<int, int> GetSalesByProduct()
        {
            return db.OrderDetail
                .GroupBy(detail => detail.ProductID)
                .Select(group => new { ProductID = group.Key, UnitsSold = group.Sum(detail => detail.Quantity) })
                .ToList()
                .ToDictionary(item => item.ProductID, item => item.UnitsSold);
        }

        private static IEnumerable<Product> OrderBySales(IEnumerable<Product> products, IDictionary<int, int> sales, bool descending)
        {
            return descending
                ? products.OrderByDescending(product => GetUnitsSold(product.ProductID, sales)).ThenBy(product => product.ProductID)
                : products.OrderBy(product => GetUnitsSold(product.ProductID, sales)).ThenBy(product => product.ProductID);
        }

        private static int GetUnitsSold(int productId, IDictionary<int, int> sales)
        {
            int sold;
            return sales.TryGetValue(productId, out sold) ? sold : 0;
        }

        private static IQueryable<Product> ApplySearch(IQueryable<Product> products, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return products;
            }

            return products.Where(product => product.ProductName.Contains(searchTerm)
                || product.ProductDecription.Contains(searchTerm)
                || product.Category.CategoryName.Contains(searchTerm));
        }

        private static IQueryable<Product> ApplyProductSort(IQueryable<Product> products, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    return products.OrderByDescending(product => product.ProductName).ThenBy(product => product.ProductID);
                case "price_asc":
                    return products.OrderBy(product => product.ProductPrice).ThenBy(product => product.ProductID);
                case "price_desc":
                    return products.OrderByDescending(product => product.ProductPrice).ThenBy(product => product.ProductID);
                default:
                    return products.OrderBy(product => product.ProductName).ThenBy(product => product.ProductID);
            }
        }

        private static string NormalizeSortOrder(string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                case "price_asc":
                case "price_desc":
                    return sortOrder;
                default:
                    return "name_asc";
            }
        }

        private static string NormalizeSearchTerm(string searchTerm)
        {
            return string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
        }

        private static int NormalizePage(int? page)
        {
            return !page.HasValue || page.Value < 1 ? 1 : page.Value;
        }

        private static int NormalizeQuantity(int? quantity)
        {
            if (!quantity.HasValue || quantity.Value < 1)
            {
                return 1;
            }

            return Math.Min(quantity.Value, MaximumCartQuantity);
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
