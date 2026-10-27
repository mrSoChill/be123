using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using demoBE.Models;
using demoBE.Models.ViewModel;
using PagedList;

namespace demoBE.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private const int ProductPageSize = 10;
        private readonly MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            string normalizedSearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
            string normalizedSortOrder = NormalizeSortOrder(sortOrder);
            int requestedPage = page.GetValueOrDefault(1);

            if (requestedPage < 1)
            {
                requestedPage = 1;
            }

            if (!string.Equals(sortOrder, normalizedSortOrder, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.Remove("sortOrder");
            }

            // A malformed page value is treated as the first page and must not block a valid search.
            if (ModelState.ContainsKey("page") && ModelState["page"].Errors.Any())
            {
                ModelState.Remove("page");
                requestedPage = 1;
            }

            var model = new ProductSearchVM
            {
                SearchTerm = normalizedSearchTerm,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortOrder = normalizedSortOrder,
                PageNumber = requestedPage,
                PageSize = ProductPageSize
            };

            if (!ValidateSearchPrices(minPrice, maxPrice))
            {
                model.PageNumber = 1;
                model.Products = new StaticPagedList<Product>(Enumerable.Empty<Product>(), 1, ProductPageSize, 0);
                return View(model);
            }

            IQueryable<Product> products = db.Product.Include(product => product.Category);

            if (!string.IsNullOrWhiteSpace(normalizedSearchTerm))
            {
                products = products.Where(product =>
                    product.ProductName.Contains(normalizedSearchTerm) ||
                    product.ProductDecription.Contains(normalizedSearchTerm) ||
                    product.Category.CategoryName.Contains(normalizedSearchTerm));
            }

            if (minPrice.HasValue)
            {
                products = products.Where(product => product.ProductPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(product => product.ProductPrice <= maxPrice.Value);
            }

            products = ApplySort(products, normalizedSortOrder);

            int totalItemCount = products.Count();
            int totalPageCount = (int)Math.Ceiling(totalItemCount / (decimal)ProductPageSize);
            if (totalPageCount > 0 && requestedPage > totalPageCount)
            {
                requestedPage = totalPageCount;
            }

            model.PageNumber = requestedPage;
            model.Products = products.ToPagedList(requestedPage, ProductPageSize);
            return View(model);
        }

        public ActionResult Details(int? id)
        {
            Product product = FindProduct(id);
            return product == null ? (ActionResult)HttpNotFound() : View(product);
        }

        public ActionResult Create()
        {
            PopulateCategories();
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage")] Product product)
        {
            ValidateProduct(product);
            if (!ModelState.IsValid)
            {
                PopulateCategories(product == null ? (int?)null : product.CategoryID);
                return View(product);
            }

            db.Product.Add(product);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã thêm sản phẩm.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            Product product = FindProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            PopulateCategories(product.CategoryID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage")] Product product)
        {
            ValidateProduct(product);
            if (!ModelState.IsValid)
            {
                PopulateCategories(product == null ? (int?)null : product.CategoryID);
                return View(product);
            }

            Product existingProduct = db.Product.Find(product.ProductID);
            if (existingProduct == null)
            {
                return HttpNotFound();
            }

            existingProduct.CategoryID = product.CategoryID;
            existingProduct.ProductName = product.ProductName;
            existingProduct.ProductDecription = product.ProductDecription;
            existingProduct.ProductPrice = product.ProductPrice;
            existingProduct.ProductImage = product.ProductImage;
            db.Entry(existingProduct).State = EntityState.Modified;
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã cập nhật sản phẩm.";
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            Product product = FindProduct(id);
            return product == null ? (ActionResult)HttpNotFound() : View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            if (db.OrderDetail.Any(detail => detail.ProductID == id))
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm đã có chi tiết đơn hàng tham chiếu.";
                return RedirectToAction("Index");
            }

            try
            {
                db.Product.Remove(product);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Đã xóa sản phẩm.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm vì dữ liệu đang được tham chiếu.";
            }

            return RedirectToAction("Index");
        }

        private Product FindProduct(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return db.Product.Include(product => product.Category)
                .SingleOrDefault(product => product.ProductID == id.Value);
        }

        private bool ValidateSearchPrices(decimal? minPrice, decimal? maxPrice)
        {
            bool isValid = true;

            if (HasBindingError("minPrice"))
            {
                ReplaceModelStateErrors("minPrice", "Giá từ phải là một số hợp lệ.");
                isValid = false;
            }

            if (HasBindingError("maxPrice"))
            {
                ReplaceModelStateErrors("maxPrice", "Giá đến phải là một số hợp lệ.");
                isValid = false;
            }

            if (minPrice.HasValue && minPrice.Value < 0)
            {
                ModelState.AddModelError("minPrice", "Giá từ không được âm.");
                isValid = false;
            }

            if (maxPrice.HasValue && maxPrice.Value < 0)
            {
                ModelState.AddModelError("maxPrice", "Giá đến không được âm.");
                isValid = false;
            }

            if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
            {
                ModelState.AddModelError(string.Empty, "Giá từ không được lớn hơn giá đến.");
                isValid = false;
            }

            return isValid;
        }

        private bool HasBindingError(string key)
        {
            return ModelState.ContainsKey(key) && ModelState[key].Errors.Any();
        }

        private void ReplaceModelStateErrors(string key, string message)
        {
            ModelState[key].Errors.Clear();
            ModelState.AddModelError(key, message);
        }

        private static string NormalizeSortOrder(string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                case "price_asc":
                case "price_desc":
                case "name_asc":
                    return sortOrder;
                default:
                    return "name_asc";
            }
        }

        private static IQueryable<Product> ApplySort(IQueryable<Product> products, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    return products.OrderByDescending(product => product.ProductName)
                        .ThenBy(product => product.ProductID);
                case "price_asc":
                    return products.OrderBy(product => product.ProductPrice)
                        .ThenBy(product => product.ProductID);
                case "price_desc":
                    return products.OrderByDescending(product => product.ProductPrice)
                        .ThenBy(product => product.ProductID);
                default:
                    return products.OrderBy(product => product.ProductName)
                        .ThenBy(product => product.ProductID);
            }
        }

        private void PopulateCategories(int? selectedCategoryId = null)
        {
            ViewBag.CategoryID = new SelectList(
                db.Category.OrderBy(category => category.CategoryName).ToList(),
                "CategoryID",
                "CategoryName",
                selectedCategoryId);
        }

        private void ValidateProduct(Product product)
        {
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu sản phẩm không hợp lệ.");
                return;
            }

            product.ProductName = (product.ProductName ?? string.Empty).Trim();
            product.ProductDecription = (product.ProductDecription ?? string.Empty).Trim();
            product.ProductImage = string.IsNullOrWhiteSpace(product.ProductImage) ? null : product.ProductImage.Trim();

            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                ModelState.AddModelError("ProductName", "Tên sản phẩm là bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(product.ProductDecription))
            {
                ModelState.AddModelError("ProductDecription", "Mô tả sản phẩm là bắt buộc.");
            }

            if (product.ProductPrice <= 0)
            {
                ModelState.AddModelError("ProductPrice", "Giá sản phẩm phải lớn hơn 0.");
            }

            if (!db.Category.Any(category => category.CategoryID == product.CategoryID))
            {
                ModelState.AddModelError("CategoryID", "Vui lòng chọn danh mục hợp lệ.");
            }
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
